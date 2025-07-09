using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wall Running")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce = 200f;
    public float wallRunJumpUpForce = 7f;
    public float wallRunJumpSideForce = 7f;
    public float maxWallRunTime = 1f;
    public float wallClimbSpeed;
    private float wallRunTimer;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;
    private float horizontalInput;
    private float verticalInput;

    [Header("Limitations")]
    public bool doJumpOnEndOfTimer = true;
    public int allowedWallJumps = 1;

    [Header("Detection")]
    public float wallCheckDistance = 0.7f;
    public float minJumpHeight = 2f;
    public float exitWallTime = 0.2f;

    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity = false;

    [Header("References")]
    public Transform orientation;
    public Transform camT;
    private PlayerMovement pm;
    private Rigidbody rb;


    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    private bool wallLeft;
    private bool wallRight;

    private bool exitingWall;

    private bool wallRemembered;
    private Transform lastWall;

    private int wallJumpsDone;



    private void Start()
    {
        if (whatIsWall.value == 0)
            whatIsWall = LayerMask.GetMask("Default");

        if (whatIsGround.value == 0)
            whatIsGround = LayerMask.GetMask("Default");

        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        /// cam = GetComponent<PlayerCam>();
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
        // Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            // start wallrun
            if (!pm.wallrunning) StartWallRun();

            // wallrun timer
            wallRunTimer -= Time.deltaTime;

            if (wallRunTimer < 0 && pm.wallrunning)
            {
                if (doJumpOnEndOfTimer)
                    WallJump();

                else
                {
                    exitingWall = true;
                    exitWallTimer = exitWallTime;
                }
            }

            // wall jump
            if (Input.GetKeyDown(jumpKey)) WallJump();
        }

        // State 2 - Exiting
        else if (exitingWall)
        {
            pm.restricted = true;

            if (pm.wallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }

        // State 3 - None
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }

        if (!exitingWall && pm.restricted)
            pm.restricted = false;
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);

        // reset readyToClimb and wallJumps whenever player hits a new wall
        if ((wallLeft || wallRight) && NewWallHit())
        {
            wallJumpsDone = 0;
            wallRunTimer = maxWallRunTime;
        }
    }
    private void RememberLastWall()
    {
        if (wallLeft)
            lastWall = leftWallHit.transform;

        if (wallRight)
            lastWall = rightWallHit.transform;
    }

    private bool NewWallHit()
    {
        if (lastWall == null)
            return true;

        if (wallLeft && leftWallHit.transform != lastWall)
            return true;

        else if (wallRight && rightWallHit.transform != lastWall)
            return true;

        return false;
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;

        // move to CheckForWall() later
        wallRunTimer = maxWallRunTime;

        rb.useGravity = useGravity;

        wallRemembered = false;

        // fov and cam tilt in full file
        /// cam.DoFov(100f);
        /// if(wallRight) cam.DoTilt(5f);
        /// if(wallLeft) cam.DoTilt(-5f);
    }

    private void WallRunningMovement()
    {
        // set gravity
        rb.useGravity = useGravity;

        // calculate directions

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        // add forces

        // forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        /// not the best way to handle this
        if (upwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        if (downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        if (!upwardsRunning && !downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (!exitingWall && !(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);

        // remember the last wall

        if (!wallRemembered)
        {
            RememberLastWall();
            wallRemembered = true;
        }
    }

    private void StopWallRun()
    {
        rb.useGravity = true;

        pm.wallrunning = false;

        /// cam.ResetFov();
        /// cam.ResetTilt();
    }

    public void WallJump()
    {
        // idea: allow one full jump, the second one is without upward force

        bool firstJump = true;

        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 forceToApply = new Vector3();

        if (wallLeft)
            forceToApply = transform.up * wallRunJumpUpForce + leftWallHit.normal * wallRunJumpSideForce;

        else if (wallRight)
            forceToApply = transform.up * wallRunJumpUpForce + rightWallHit.normal * wallRunJumpSideForce;

        firstJump = wallJumpsDone < allowedWallJumps;
        wallJumpsDone++;

        // if not first jump, remove y component of force
        if (!firstJump)
            forceToApply = new Vector3(forceToApply.x, 0f, forceToApply.z);

        // add force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
        // rb.AddForce(orientation.forward * 1f, ForceMode.Impulse);

        RememberLastWall();

        // stop wallRun
        StopWallRun();
    }
}