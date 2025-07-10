using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("References")]
    public Transform playerCam;
    private PlayerMovement pm;
    private Rigidbody rb;

    [Header("Wall Running")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce = 200f;
    public float wallRunJumpUpForce = 7f;
    public float wallRunJumpSideForce = 7f;
    public float maxWallRunTime = 1f;
    public float wallClimbSpeed = 3f;
    public bool useGravity = false;
    public bool doJumpOnEndOfTimer = true;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;

    [Header("Detection")]
    public float wallCheckDistance = 0.7f;
    public float minJumpHeight = 2f;
    public float exitWallTime = 0.2f;

    private bool upwardsRunning, downwardsRunning;
    private float horizontalInput, verticalInput;
    private float wallRunTimer, exitWallTimer;
    private bool wallLeft, wallRight, exitingWall, wallRemembered;
    private RaycastHit leftWallHit, rightWallHit;
    private Transform lastWall;
    private int wallJumpsDone;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        if (playerCam == null)
            playerCam = GameObject.Find("PlayerCamera")?.transform;
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();

        if (pm.grounded && lastWall != null)
            lastWall = null;
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning && !exitingWall)
            WallRunningMovement();
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallrunning) StartWallRun();

            wallRunTimer -= Time.deltaTime;

            if (wallRunTimer < 0 && pm.wallrunning)
            {
                if (doJumpOnEndOfTimer) WallJump();
                else { exitingWall = true; exitWallTimer = exitWallTime; }
            }

            if (Input.GetKeyDown(jumpKey)) WallJump();
        }
        else if (exitingWall)
        {
            pm.restricted = true;
            if (pm.wallrunning) StopWallRun();

            exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0) exitingWall = false;
        }
        else if (pm.wallrunning)
        {
            StopWallRun();
        }

        if (!exitingWall && pm.restricted)
            pm.restricted = false;
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallCheckDistance, whatIsWall);

        if ((wallLeft || wallRight) && NewWallHit())
        {
            wallJumpsDone = 0;
            wallRunTimer = maxWallRunTime;
        }
    }

    private bool AboveGround() => !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);

    private void StartWallRun()
    {
        pm.wallrunning = true;
        rb.useGravity = useGravity;
        wallRunTimer = maxWallRunTime;
        wallRemembered = false;
    }

    private void StopWallRun()
    {
        rb.useGravity = true;
        pm.wallrunning = false;
    }

    public void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 forceToApply = wallLeft
            ? Vector3.up * wallRunJumpUpForce + leftWallHit.normal * wallRunJumpSideForce
            : Vector3.up * wallRunJumpUpForce + rightWallHit.normal * wallRunJumpSideForce;

        if (wallJumpsDone >= 1)
            forceToApply = new Vector3(forceToApply.x, 0f, forceToApply.z);

        wallJumpsDone++;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        RememberLastWall();
        StopWallRun();
    }

    private void WallRunningMovement()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);
        if ((transform.forward - wallForward).magnitude > (transform.forward + wallForward).magnitude)
            wallForward = -wallForward;

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (upwardsRunning) rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        if (downwardsRunning) rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        if (!upwardsRunning && !downwardsRunning) rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (!exitingWall && !(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);

        if (!wallRemembered)
        {
            RememberLastWall();
            wallRemembered = true;
        }
    }

    private void RememberLastWall()
    {
        if (wallLeft) lastWall = leftWallHit.transform;
        if (wallRight) lastWall = rightWallHit.transform;
    }

    private bool NewWallHit()
    {
        if (lastWall == null) return true;
        if (wallLeft && leftWallHit.transform != lastWall) return true;
        if (wallRight && rightWallHit.transform != lastWall) return true;
        return false;
    }
}
