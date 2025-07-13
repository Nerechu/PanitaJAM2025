using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ParkourFPS
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SoundPlayer))]
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerControllerScript : MonoBehaviour
    {
        #region Components
        private CapsuleCollider capsuleCollider;
        private Rigidbody playerRigidbody;
        private SoundPlayer soundPlayer;
        private PlayerMovement playerMovement;
        private Camera cameraComponent;
        #endregion

        #region Camera
        [Header("Camera")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private GameObject speedLines;
        [SerializeField] private float fieldOfView = 80;
        [SerializeField] private float lookSensitivity = 2;
        private float currRotationX = 0;
        private const float lookXLimit = 45;
        #endregion

        #region Movement Speeds
        [Header("Walking")]
        [SerializeField] private float walkSpeed = 100;
        [SerializeField] private float airMultiplier = 1.1f;
        [SerializeField] private bool slopeSpeedDecrease = false;

        [Header("Running")]
        [SerializeField] private bool runningEnabled = true;
        [SerializeField] private KeyCode runButton = KeyCode.LeftShift;
        [SerializeField] private float runSpeed = 130;
        [SerializeField] private float runFovIncrease = 10;
        private bool isRunning;

        [Header("Crouch & Slide")]
        [SerializeField] private bool crouchingEnabled = true;
        [SerializeField] private KeyCode crouchButton = KeyCode.LeftControl;
        [SerializeField] private float crouchSpeed = 80;
        [SerializeField] private bool slidingEnabled = true;
        [SerializeField] private KeyCode slideButton = KeyCode.C;
        [SerializeField] private float slideDuration = 0.5f;
        [SerializeField] private float slideFovIncrease = 3;
        private bool isSliding;
        private bool isCrouching;
        private bool changedPlayerHeight;
        #endregion

        #region Jumping
        [Header("Jumping")]
        [SerializeField] private bool jumpingEnabled = true;
        [SerializeField] private KeyCode jumpButton = KeyCode.Space;
        [SerializeField] private float jumpAmount = 80;
        #endregion

        #region Wallrunning
        [Header("Wallrunning")]
        [SerializeField] private bool wallrunningEnabled = true;
        [SerializeField] private float wallRunGravityBoost = 120f;
        [SerializeField] private float wallRunForwardBoost = 30f;
        [SerializeField] private float maxWallRunTime = 2f;
        [SerializeField] private int wallRunCameraLean = 15;
        private bool isWallrunning;
        private float wallRunTimer;
        #endregion

        #region Momentum
        [Header("Momentum")]
        [SerializeField] private bool useMomentum = true;
        [SerializeField] private Text momentumText;
        [SerializeField] private float momentumResetThreshold = 3f;
        [SerializeField] private float momentumDecreaseRate = 0.015f;
        [SerializeField] private float runMomentumIncrease = 0.002f;
        [SerializeField] private float slideMomentumIncrease = 0.2f;
        [SerializeField] private float wallRunMomentumIncrease = 0.01f;
        private float momentum = 1f;
        #endregion

        #region Stamina
        [Header("Stamina")]
        [SerializeField] private bool staminaEnabled = true;
        [SerializeField] private Text staminaText;
        [SerializeField] private float staminaDuration = 10;
        [SerializeField] private float staminaFillRate = 2;
        private float currStamina;
        private bool staminaEmpty;
        #endregion

        #region Gravity & Drag
        [Header("Gravity & Drag")]
        [SerializeField] private float gravity = 400;
        [SerializeField] private float fallMultiplier = 1.5f;
        [SerializeField] private float groundDrag = 5;
        [SerializeField] private float airDrag = 5;
        #endregion

        #region Ground & Wall Check
        [Header("Ground & Wall Check")]
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform wallCheckRight;
        [SerializeField] private Transform wallCheckLeft;
        private const float groundDistance = 0.4f;
        private bool isGrounded;
        private bool touchingWallRight, touchingWallLeft;
        private RaycastHit slopeHit;
        #endregion

        public bool IsGrounded() => isGrounded;

        private void Start()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            playerRigidbody = GetComponent<Rigidbody>();
            soundPlayer = GetComponent<SoundPlayer>();
            playerMovement = GetComponent<PlayerMovement>();
            cameraComponent = cameraTransform.GetComponent<Camera>();

            cameraComponent.fieldOfView = fieldOfView;
            playerRigidbody.useGravity = false;

            if (staminaEnabled) StartCoroutine(ControlStamina());
            if (useMomentum)
            {
                StartCoroutine(ReduceMomentum());
                StartCoroutine(CheckMomentumReset());
            }
        }

        private void Update()
        {
            SetRotation();
            HandleInput();

            if (isWallrunning)
            {
                wallRunTimer += Time.deltaTime;
                if (wallRunTimer >= maxWallRunTime)
                {
                    isWallrunning = false;
                    SetSpeedLines(false);
                }
            }
            else
            {
                wallRunTimer = 0f;
            }
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            CheckWallStatus();
            SetDrag();
            SetFov();
            SetPlayerHeight();
            ApplyCustomGravity();

            if (!(playerMovement.dashing || playerMovement.climbing || playerMovement.swinging))
                MovePlayer();
        }

        private void ApplyCustomGravity()
        {
            if (isWallrunning)
            {
                playerRigidbody.AddForce(Vector3.up * wallRunGravityBoost, ForceMode.Acceleration);
                playerRigidbody.AddForce(transform.forward * wallRunForwardBoost, ForceMode.Acceleration);
            }
            else if (!isGrounded)
            {
                float adjustedGravity = gravity;
                if (playerRigidbody.velocity.y < 0)
                    adjustedGravity *= fallMultiplier;
                playerRigidbody.AddForce(Vector3.down * adjustedGravity, ForceMode.Acceleration);
            }
        }

        private void MovePlayer()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            if (CanWallRun())
            {
                isWallrunning = true;
                SetSpeedLines(true);
                momentum += wallRunMomentumIncrease;
                h *= 0.3f;
            }
            else
            {
                isWallrunning = false;
                SetSpeedLines(false);
            }

            if (!isWallrunning && ((touchingWallRight && h > 0) || (touchingWallLeft && h < 0)))
                h = 0;

            Vector3 moveDir = transform.forward * v + transform.right * h;
            Vector3 force = moveDir.normalized;

            if (!isGrounded) force *= airMultiplier;
            else if (OnSlope())
            {
                Vector3 slopeDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);
                force = slopeDir.normalized;
                if (!slopeSpeedDecrease) force *= (1 + slopeDir.y);
            }

            if (isRunning)
            {
                momentum += runMomentumIncrease;
                force *= runSpeed;
            }
            else if (isCrouching) force *= crouchSpeed;
            else force *= walkSpeed;

            if (useMomentum) force *= momentum;

            playerRigidbody.AddForce(force, ForceMode.Acceleration);
        }

        private void HandleInput()
        {
            if (Input.GetKey(runButton) && runningEnabled && (!staminaEnabled || !staminaEmpty))
            {
                isRunning = true;
                isCrouching = false;
            }
            else
            {
                isRunning = false;
                isCrouching = Input.GetKey(crouchButton);
            }

            if (Input.GetKeyDown(jumpButton) && jumpingEnabled)
                Jump();

            if (Input.GetKeyDown(slideButton) && slidingEnabled && !isSliding)
                StartCoroutine(Slide());
        }

        private void Jump()
        {
            if (isGrounded || isWallrunning)
            {
                Vector3 jumpDir = Vector3.up;

                if (isWallrunning)
                {
                    if (touchingWallRight)
                        jumpDir += -transform.right;
                    else if (touchingWallLeft)
                        jumpDir += transform.right;
                }

                playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z);
                playerRigidbody.AddForce(jumpDir.normalized * jumpAmount, ForceMode.Impulse);

                isGrounded = false;
                isWallrunning = false;
                SetSpeedLines(false);

                soundPlayer.PlaySound(soundPlayer.jumpSound, volume: 0.6f);
            }
        }

        private IEnumerator Slide()
        {
            isSliding = true;
            SetSpeedLines(true);
            if (slideMomentumIncrease != 0) momentum += slideMomentumIncrease;
            soundPlayer.PlaySound(soundPlayer.slidingSound);
            yield return new WaitForSeconds(slideDuration);
            isSliding = false;
            SetSpeedLines(false);
        }

        public void SetSpeedLines(bool enabled)
        {
            if (speedLines != null)
                speedLines.SetActive(enabled);
        }

        private void SetRotation()
        {
            currRotationX -= Input.GetAxis("Mouse Y") * lookSensitivity;
            currRotationX = Mathf.Clamp(currRotationX, -lookXLimit, lookXLimit);
            float lean = isWallrunning ? wallRunCameraLean * (touchingWallLeft ? -1 : 1) : 0;
            cameraTransform.localRotation = Quaternion.Euler(currRotationX, 0, lean);
            transform.Rotate(0, Input.GetAxis("Mouse X") * lookSensitivity, 0);
        }

        private void SetDrag()
        {
            playerRigidbody.drag = isGrounded ? groundDrag : airDrag;
        }

        private void SetFov()
        {
            float targetFov = fieldOfView;
            if (isRunning) targetFov += runFovIncrease;
            else if (isSliding) targetFov += slideFovIncrease;
            cameraComponent.fieldOfView = targetFov;
        }

        private void SetPlayerHeight()
        {
            if ((isSliding || isCrouching) && !changedPlayerHeight)
            {
                capsuleCollider.height /= 2f;
                capsuleCollider.center -= new Vector3(0, capsuleCollider.height / 2f, 0);
                cameraTransform.localPosition /= 4f;
                changedPlayerHeight = true;
            }
            else if (!(isSliding || isCrouching) && changedPlayerHeight)
            {
                capsuleCollider.center += new Vector3(0, capsuleCollider.height / 2f, 0);
                capsuleCollider.height *= 2f;
                cameraTransform.localPosition *= 4f;
                changedPlayerHeight = false;
            }
        }

        private bool OnSlope()
        {
            return Physics.Raycast(groundCheck.position, Vector3.down, out slopeHit, groundDistance) &&
                   slopeHit.normal != Vector3.up;
        }

        private void CheckGrounded()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }

        private void CheckWallStatus()
        {
            touchingWallRight = Physics.CheckSphere(wallCheckRight.position, groundDistance * 2f, groundMask);
            touchingWallLeft = Physics.CheckSphere(wallCheckLeft.position, groundDistance * 2f, groundMask);
        }

        private bool CanWallRun()
        {
            return wallrunningEnabled && (touchingWallRight || touchingWallLeft); // ✅ Opción 1: sin !isGrounded
        }

        private IEnumerator ControlStamina()
        {
            currStamina = staminaDuration;
            while (true)
            {
                if (isRunning && currStamina > 0) currStamina -= 0.1f;
                else if (currStamina < staminaDuration) currStamina += staminaFillRate * 0.1f;
                if (staminaText) staminaText.text = $"Stamina: {currStamina:F1}";
                staminaEmpty = currStamina <= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator ReduceMomentum()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (momentum > 1f) momentum -= momentumDecreaseRate;
            }
        }

        private IEnumerator CheckMomentumReset()
        {
            while (true)
            {
                if (playerRigidbody.velocity.magnitude < momentumResetThreshold)
                    momentum = 1f;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
