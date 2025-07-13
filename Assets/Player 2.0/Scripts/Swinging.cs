using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Swinging : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform gunTip;
    public Rigidbody rb;
    public LayerMask grappleMask;

    [Header("Grapple Settings")]
    public float maxDistance = 50f;
    public float boostSpeed = 75f;
    public float stopDistance = 3f;
    public float grappleCooldown = 1f;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    [Header("Visual")]
    public LineRenderer lineRenderer;

    private Vector3 grapplePoint;
    private bool isGrappling;
    private bool isFlying;
    private float cooldownTimer;

    private PlayerMovement pm;

    private void Awake()
    {
        pm = GetComponent<PlayerMovement>();
        if (cam == null) cam = Camera.main?.transform;
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(grappleKey) && cooldownTimer <= 0f)
            TryStartGrapple();

        if (isFlying)
        {
            Vector3 dir = (grapplePoint - transform.position).normalized;
            rb.velocity = dir * boostSpeed;

            float distance = Vector3.Distance(transform.position, grapplePoint);
            if (distance < stopDistance || Input.GetKeyUp(grappleKey))
                EndGrapple();
        }

        if (isGrappling)
            UpdateRope();
    }

    private void TryStartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, grappleMask))
        {
            grapplePoint = hit.point;
            isFlying = true;
            isGrappling = true;
            cooldownTimer = grappleCooldown;

            pm.swinging = true; // si querés bloquear input en el aire
            SetupLine();
        }
    }

    private void EndGrapple()
    {
        isFlying = false;
        isGrappling = false;
        pm.swinging = false;

        if (lineRenderer)
        {
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }

        // Impulso extra al soltarse
        Vector3 launchDir = (grapplePoint - transform.position).normalized + Vector3.up * 0.5f;
        rb.velocity = launchDir * boostSpeed;
    }

    private void SetupLine()
    {
        if (lineRenderer == null)
        {
            GameObject obj = new GameObject("GrappleLine");
            lineRenderer = obj.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.05f;
        }

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;
    }

    private void UpdateRope()
    {
        if (!lineRenderer || !gunTip) return;
        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}
