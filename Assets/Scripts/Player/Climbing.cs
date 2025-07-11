using UnityEngine;

public class Climbing : MonoBehaviour
{
    public Rigidbody rb;
    public LayerMask whatIsWall;
    public PlayerMovement pm;
    public Transform playerCam;

    public float climbSpeed = 5f;
    public float wallRunSpeed = 3f;
    public float maxClimbTime = 3f;
    private float climbTimer;

    public float detectionLength = 1f;
    public float sphereCastRadius = 0.5f;
    public float maxWallLookAngle = 45f;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront, climbing;

    private void Start()
    {
        if (playerCam == null)
            playerCam = GameObject.Find("PlayerCamera")?.transform;
    }

    private void Update()
    {
        WallCheck();
        StateMachine();

        if (climbing)
            ClimbingMovement();
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, transform.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(transform.forward, -frontWallHit.normal);

        if (pm.grounded)
            climbTimer = maxClimbTime;
    }

    private void StateMachine()
    {
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle)
        {
            if (!climbing && climbTimer > 0)
                StartClimbing();

            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            else StopClimbing();
        }
        else if (climbing)
        {
            StopClimbing();
        }
    }

    private void StartClimbing()
    {
        climbing = true;
        pm.climbing = true;
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x * wallRunSpeed, climbSpeed, rb.velocity.z * wallRunSpeed);
    }

    private void StopClimbing()
    {
        climbing = false;
        pm.climbing = false;
    }
}
