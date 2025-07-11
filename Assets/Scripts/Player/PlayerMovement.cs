using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float groundDrag = 4f;

    public float jumpForce = 12f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.6f;
    public float coyoteTime = 0.2f;
    private float coyoteTimeTimer;

    public float wallrunSpeed = 12f;
    public float dashSpeed = 18f;
    public float dashSpeedChangeFactor = 5f;
    public float maxYSpeed = 0;
    public float climbSpeed = 4f;
    public float swingSpeed = 10f;
    public float airMinSpeed = 2f;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    public bool grounded;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;

    public MovementState state;
    public enum MovementState { walking, sprinting, air, wallrunning, restricted, dashing, climbing, swinging }

    public bool wallrunning, restricted, dashing, climbing, swinging;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;

    private bool readyToJump;

    private float speedChangeFactor = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        readyToJump = true;
        coyoteTimeTimer = coyoteTime;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        HandleInput();
        HandleState();
        SpeedControl();

        rb.drag = (grounded && (state == MovementState.walking || state == MovementState.sprinting)) ? groundDrag : 0;
    }

    private void FixedUpdate()
    {
        if (state != MovementState.restricted)
            MovePlayer();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && readyToJump && (grounded || coyoteTimeTimer > 0))
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (state == MovementState.walking && (horizontalInput != 0 || verticalInput != 0))
        {
            if (!AudioManager.instance.audioSource.isPlaying)
                AudioManager.instance.PlaySound(SoundType.WALK);
        }

        else if (state == MovementState.sprinting && (horizontalInput != 0 || verticalInput != 0))
        {
            if (!AudioManager.instance.audioSource.isPlaying)
                AudioManager.instance.PlaySound(SoundType.RUN);
        }


    }

    private void MovePlayer()
    {
        if (state == MovementState.dashing || state == MovementState.swinging)
            return;

        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        moveDirection.Normalize();

        if (grounded)
            rb.AddForce(moveDirection * moveSpeed, ForceMode.VelocityChange);
        else
            rb.AddForce(moveDirection * moveSpeed * airMultiplier, ForceMode.Force);
    }

    private void Jump()
    {
        coyoteTimeTimer = 0;
        StopAllCoroutines();
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Jump sound

       AudioManager.instance.audioSource.Stop();
       AudioManager.instance.PlaySound(SoundType.JUMP);
    }

    private void ResetJump() => readyToJump = true;

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
    }

    private void HandleState()
    {
        if (restricted) { state = MovementState.restricted; return; }

        if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
            coyoteTimeTimer = 0;
        }
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
            coyoteTimeTimer = 0;
        }
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
            if (readyToJump) coyoteTimeTimer = coyoteTime;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            if (readyToJump) coyoteTimeTimer = coyoteTime;
        }
        else if (swinging)
        {
            state = MovementState.swinging;
            desiredMoveSpeed = swingSpeed;
            coyoteTimeTimer = 0;
        }
        else
        {
            state = MovementState.air;
            desiredMoveSpeed = desiredMoveSpeed < sprintSpeed ? walkSpeed : sprintSpeed;
        }

        if (desiredMoveSpeed != lastDesiredMoveSpeed)
        {
            if (lastState == MovementState.dashing)
                keepMomentum = true;

            StopAllCoroutines();

            if (keepMomentum)
                StartCoroutine(SmoothlyLerpMoveSpeed());
            else
                moveSpeed = desiredMoveSpeed;
        }

        if ((lastState == MovementState.walking || lastState == MovementState.sprinting) && state == MovementState.air)
            StartCoroutine(CoyoteTime());

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime * speedChangeFactor;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    private IEnumerator CoyoteTime()
    {
        while (coyoteTimeTimer > 0)
        {
            coyoteTimeTimer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
