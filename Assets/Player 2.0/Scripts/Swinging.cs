// === Optimized and Corrected Swinging.cs ===
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
public class Swinging : MonoBehaviour
{
    [Header("References")]
    public Transform gunTip;
    public Transform cam;
    public Rigidbody rb;
    public LayerMask whatIsGrappleable;

    private PlayerMovement pm;
    private LineRenderer lr;
    private SpringJoint joint;

    [Header("Swinging Settings")]
    public float maxSwingDistance = 25f;
    public float spring = 4.5f;
    public float damper = 7f;
    public float massScale = 4.5f;

    [Header("Movement")]
    public float horizontalThrustForce = 10f;
    public float forwardThrustForce = 15f;
    public float extendCableSpeed = 2f;

    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse1;

    [Header("Effects")]
    public GameObject hookFailEffectPrefab;
    public Transform grappleGunModel;
    public Vector3 recoilOffset = new Vector3(0, -0.1f, -0.2f);

    private Vector3 swingPoint;
    private Vector3 realHitPoint;
    private Vector3 currentGrapplePosition;
    private Vector3 originalGunPosition;
    private bool isAttemptingSwing;

    private void Awake()
    {
        pm = GetComponent<PlayerMovement>();
        if (cam == null)
            cam = Camera.main?.transform;

        if (grappleGunModel != null)
            originalGunPosition = grappleGunModel.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(swingKey)) AttemptSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();

        if (isAttemptingSwing && joint == null)
            UpdateRopeToMouseDirection();

        if (joint != null)
            ApplyOdmGearMovement();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void AttemptSwing()
    {
        if (gunTip == null || cam == null) return;

        RaycastHit hit;
        currentGrapplePosition = gunTip.position;
        SetupLineRenderer();

        bool hitSomething = Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance);
        realHitPoint = hitSomething ? hit.point : cam.position + cam.forward * maxSwingDistance;
        isAttemptingSwing = true;

        if (grappleGunModel != null)
        {
            StopAllCoroutines();
            StartCoroutine(PlayGrappleRecoil());
        }

        if (hitSomething && ((1 << hit.collider.gameObject.layer) & whatIsGrappleable) != 0)
        {
            swingPoint = hit.point;
            pm.swinging = true;

            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float dist = Vector3.Distance(transform.position, swingPoint);
            joint.maxDistance = dist * 0.8f;
            joint.minDistance = dist * 0.25f;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;
        }
        else if (hitSomething && hookFailEffectPrefab)
        {
            Instantiate(hookFailEffectPrefab, hit.point, Quaternion.LookRotation(-cam.forward));
            Invoke(nameof(StopSwing), 0.2f);
        }
        else
        {
            Invoke(nameof(StopSwing), 0.2f);
        }
    }

    private IEnumerator PlayGrappleRecoil()
    {
        Vector3 target = originalGunPosition + recoilOffset;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            grappleGunModel.localPosition = Vector3.Lerp(originalGunPosition, target, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            grappleGunModel.localPosition = Vector3.Lerp(target, originalGunPosition, t);
            yield return null;
        }

        grappleGunModel.localPosition = originalGunPosition;
    }

    public void StopSwing()
    {
        isAttemptingSwing = false;
        pm.swinging = false;

        if (joint) Destroy(joint);
        if (lr)
        {
            lr.positionCount = 0;
            Destroy(lr.gameObject);
            lr = null;
        }
    }

    private void ApplyOdmGearMovement()
    {
        if (Input.GetKey(KeyCode.D)) rb.AddForce(transform.right * horizontalThrustForce, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.A)) rb.AddForce(-transform.right * horizontalThrustForce, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.W)) rb.AddForce(transform.forward * horizontalThrustForce, ForceMode.Acceleration);

        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 dir = swingPoint - transform.position;
            rb.AddForce(dir.normalized * forwardThrustForce, ForceMode.Acceleration);

            float dist = Vector3.Distance(transform.position, swingPoint);
            joint.maxDistance = dist * 0.8f;
            joint.minDistance = dist * 0.25f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            float dist = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;
            joint.maxDistance = dist * 0.8f;
            joint.minDistance = dist * 0.25f;
        }
    }

    private void DrawRope()
    {
        if (!isAttemptingSwing || lr == null) return;

        if (joint != null)
        {
            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, currentGrapplePosition);
        }
        else
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, realHitPoint);
        }
    }

    private void SetupLineRenderer()
    {
        if (lr != null) return;

        GameObject lrObj = new GameObject("GrappleLine");
        lr = lrObj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.numCapVertices = 8;
        lr.positionCount = 2;
    }

    private void UpdateRopeToMouseDirection()
    {
        if (lr == null || cam == null || gunTip == null) return;
        realHitPoint = cam.position + cam.forward * maxSwingDistance;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, realHitPoint);
    }
}
