using UnityEngine;
using System.Collections;

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
    public GameObject hitParticlesValid;
    public GameObject hitParticlesInvalid;
    public GameObject missParticles;
    public int ropeSegments = 20;
    public float ropeAnimationSpeed = 5f;

    private Vector3 grapplePoint;
    private bool isGrappling;
    private bool isFlying;
    private float cooldownTimer;
    private Coroutine ropeAnimationCoroutine;

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
        cooldownTimer = grappleCooldown;

        AudioManager.instance.PlaySound(SoundType.FIREHOOK);

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance))
        {
            grapplePoint = hit.point;
            bool validLayer = ((1 << hit.collider.gameObject.layer) & grappleMask) != 0;

            if (validLayer)
            {
                if (hitParticlesValid)
                    Instantiate(hitParticlesValid, hit.point, Quaternion.identity);

                AudioManager.instance.PlayDelayedSound(SoundType.HOOKLANDED, 1, 0.5f);

                isFlying = true;
                isGrappling = true;
                pm.swinging = true;

                StartRopeAnimation(gunTip.position, grapplePoint, false);
            }
            else
            {
                if (hitParticlesInvalid)
                    Instantiate(hitParticlesInvalid, hit.point, Quaternion.identity);

                AudioManager.instance.PlayDelayedSound(SoundType.HOOKMISSED, 1, 0.75f);

                StartRopeAnimation(gunTip.position, hit.point, true);
            }
        }
        else
        {
            Vector3 endPoint = cam.position + cam.forward * maxDistance;

            if (missParticles)
                Instantiate(missParticles, endPoint, Quaternion.identity);

            AudioManager.instance.PlayDelayedSound(SoundType.HOOKMISSED, 1, 0.75f);

            StartRopeAnimation(gunTip.position, endPoint, true);
        }
    }

    private void EndGrapple()
    {
        isFlying = false;
        isGrappling = false;
        pm.swinging = false;

        StartRopeAnimation(grapplePoint, gunTip.position, true);

        Vector3 launchDir = (grapplePoint - transform.position).normalized + Vector3.up * 0.5f;
        rb.velocity = launchDir * boostSpeed;

        AudioManager.instance.PlaySound(SoundType.HOOKRELEASE);
    }

    private void StartRopeAnimation(Vector3 start, Vector3 end, bool returnAfter)
    {
        if (ropeAnimationCoroutine != null)
            StopCoroutine(ropeAnimationCoroutine);

        ropeAnimationCoroutine = StartCoroutine(AnimateCurvedRope(start, end, returnAfter));
    }

    private IEnumerator AnimateCurvedRope(Vector3 start, Vector3 end, bool returnAfter)
    {
        float t = 0f;
        lineRenderer.enabled = true;
        lineRenderer.positionCount = ropeSegments + 1;

        while (t < 1f)
        {
            t += Time.deltaTime * ropeAnimationSpeed;
            DrawCurvedRope(start, end, Mathf.Clamp01(t));
            yield return null;
        }

        DrawCurvedRope(start, end, 1f);

        if (returnAfter)
        {
            yield return new WaitForSeconds(0.2f);
            ResetLine();
        }
    }

    private void DrawCurvedRope(Vector3 start, Vector3 end, float progress)
    {
        for (int i = 0; i <= ropeSegments; i++)
        {
            float t = i / (float)ropeSegments;
            Vector3 point = Vector3.Lerp(start, end, t);

            // Simula curvatura vertical en el medio
            float curve = Mathf.Sin(t * Mathf.PI) * 2f; // altura del arco
            Vector3 upOffset = Vector3.up * curve * (1 - progress); // más recto a medida que progresa

            lineRenderer.SetPosition(i, point + upOffset);
        }
    }

    private void UpdateRope()
    {
        DrawCurvedRope(gunTip.position, grapplePoint, 1f);
    }

    private void ResetLine()
    {
        if (lineRenderer)
        {
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }
    }
}
