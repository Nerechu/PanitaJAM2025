// === Optimized Climbing.cs ===
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Rigidbody))]
public class Climbing : MonoBehaviour
{
    [Header("Wall Detection")]
    public LayerMask whatIsWall;
    public float detectionLength = 1f;
    public float sphereCastRadius = 0.5f;
    public float maxWallLookAngle = 45f;

    [Header("Climb Settings")]
    public float climbSpeed = 5f;
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

        if (playerCam == null)
            playerCam = Camera.main?.transform;
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
        wallFront = Physics.SphereCast(
            transform.position,
            sphereCastRadius,
            transform.forward,
            out frontWallHit,
            detectionLength,
            whatIsWall
        );

        if (wallFront)
            wallLookAngle = Vector3.Angle(transform.forward, -frontWallHit.normal);
        else
            wallLookAngle = 999f;

        if (pm.grounded)
            climbTimer = maxClimbTime;
    }

    private void HandleClimbingState()
    {
        bool isLookingAtWall = wallLookAngle < maxWallLookAngle;
        bool isTryingToClimb = wallFront && Input.GetKey(KeyCode.W) && isLookingAtWall;

        if (isTryingToClimb)
        {
            if (!climbing && climbTimer > 0f)
                StartClimb();

            if (climbTimer > 0f)
                climbTimer -= Time.deltaTime;
            else
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
    }

    private void StopClimb()
    {
        climbing = false;
        pm.climbing = false;
    }

    private void Climb()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }
}
