using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Rigidbody))]
public class Climbing : MonoBehaviour
{
    [Header("Wall Detection")]
    [Tooltip("Layer that represents climbable surfaces")]
    public LayerMask whatIsWall; // En el inspector, selecciona solo "Climb"
    public float detectionLength = 1f;
    public float sphereCastRadius = 0.5f;
    public float maxWallLookAngle = 45f;

    [Header("Climb Settings")]
    public float climbSpeed = 3f;
    public float maxClimbTime = 3f;

    [Header("Camera")]
    public Transform playerCam;

    private float climbTimer;
    private bool wallFront;
    private bool climbing;
    private float wallLookAngle;
    private RaycastHit frontWallHit;

    private PlayerMovement pm;
    private Rigidbody rb;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();

        if (playerCam == null && Camera.main != null)
            playerCam = Camera.main.transform;

        climbTimer = maxClimbTime;
    }

    private void Update()
    {
        WallCheck();
        HandleClimbingState();

        if (climbing)
            Climb();
    }

    private void WallCheck()
    {
        Vector3 origin = transform.position + Vector3.up * 1.2f; // Altura del pecho

        wallFront = Physics.SphereCast(
            origin,
            sphereCastRadius,
            transform.forward,
            out frontWallHit,
            detectionLength,
            whatIsWall // Usamos Layer "Climb"
        );

        if (wallFront)
        {
            wallLookAngle = Vector3.Angle(transform.forward, -frontWallHit.normal);
        }
        else
        {
            wallLookAngle = 999f;
        }

        if (pm.grounded && !climbing)
            climbTimer = maxClimbTime;
    }

    private void HandleClimbingState()
    {
        bool isLookingAtWall = wallLookAngle < maxWallLookAngle;
        bool isTryingToClimb = wallFront && Input.GetKey(KeyCode.W) && isLookingAtWall;

        if (isTryingToClimb && climbTimer > 0f)
        {
            if (!climbing)
                StartClimb();

            climbTimer -= Time.deltaTime;
            if (climbTimer <= 0f)
                StopClimb();
        }
        else if (climbing)
        {
            StopClimb();
        }
    }

    private void StartClimb()
    {
        climbing = true;
        pm.climbing = true;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }

    private void StopClimb()
    {
        climbing = false;
        pm.climbing = false;

        rb.useGravity = true;
    }

    private void Climb()
    {
        rb.velocity = new Vector3(0f, climbSpeed, 0f);
        //Audio

        if (!AudioManager.instance.audioSource.isPlaying)
            AudioManager.instance.PlaySound(SoundType.CLIMB);
    }
}