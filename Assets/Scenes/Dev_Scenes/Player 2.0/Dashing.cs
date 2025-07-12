// === Optimized Dashing.cs ===
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Rigidbody))]
public class Dashing : MonoBehaviour
{
    [Header("References")]
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Dashing Settings")]
    public float dashForce = 20f;
    public float dashUpwardForce = 2f;
    public float maxDashYSpeed = 25f;
    public float dashDuration = 0.25f;

    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVelocity = true;

    [Header("Cooldown")]
    public float dashCooldown = 1f;
    private float dashCooldownTimer;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.E;

    private Vector3 delayedForceToApply;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        if (playerCam == null)
            playerCam = Camera.main?.transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(dashKey))
            AttemptDash();

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
    }

    private void AttemptDash()
    {
        if (dashCooldownTimer > 0 || pm.dashing) return;

        dashCooldownTimer = dashCooldown;
        pm.dashing = true;

        Transform refTransform = useCameraForward ? playerCam : transform;
        Vector3 dir = GetDashDirection(refTransform);
        Vector3 force = dir * dashForce + Vector3.up * dashUpwardForce;

        if (disableGravity) rb.useGravity = false;

        delayedForceToApply = force;
        Invoke(nameof(ApplyDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);
    }

    private void ApplyDashForce()
    {
        if (resetVelocity)
        {
            Vector3 currentVelocity = rb.velocity;
            rb.velocity = new Vector3(0, Mathf.Clamp(currentVelocity.y, -maxDashYSpeed, maxDashYSpeed), 0);
        }

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        pm.dashing = false;
        if (disableGravity) rb.useGravity = true;
    }

    private Vector3 GetDashDirection(Transform reference)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = allowAllDirections ? (reference.forward * v + reference.right * h) : reference.forward;
        return (h == 0 && v == 0) ? reference.forward : dir.normalized;
    }
}
