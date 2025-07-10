using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lr;
    public Transform gunTip;
    [SerializeField] private Transform cam;
    public Transform player;
    public LayerMask whatIsGrappleable;
    public PlayerMovement pm;

    [Header("Swinging")]
    public float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;
    private Vector3 realHitPoint;

    public float spring;
    public float damper;
    public float massScale;

    [Header("OdmGear")]
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;

    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse1;

    [Header("Effects")]
    [SerializeField] private GameObject hookFailEffectPrefab;

    private Vector3 currentGrapplePosition;
    private bool isAttemptingSwing;

    private void Awake()
    {
        if (cam == null)
            cam = transform.Find("PlayerCamera");
    }

    private void Update()
    {
        if (Input.GetKeyDown(swingKey)) AttemptSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();

        if (isAttemptingSwing && joint == null)
            UpdateLineToMouseDirection();

        if (joint != null)
            OdmGearMovement();
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Vector3 testPos = transform.position + transform.forward * 2f;
                GameObject effect = Instantiate(hookFailEffectPrefab, testPos, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }

    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void AttemptSwing()
    {
        RaycastHit hit;
        currentGrapplePosition = gunTip.position;
        SetupLineRenderer();

        bool hitSomething = Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance);

        if (hitSomething)
        {
            realHitPoint = hit.point;

            // Dibujar línea visualmente, aunque no se enganche
            isAttemptingSwing = true;

            // ¿Es hookeable?
            if (((1 << hit.collider.gameObject.layer) & whatIsGrappleable) != 0)
            {
                swingPoint = hit.point;
                pm.swinging = true;

                joint = player.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = swingPoint;

                float distance = Vector3.Distance(player.position, swingPoint);
                joint.maxDistance = distance * 0.8f;
                joint.minDistance = distance * 0.25f;

                joint.spring = spring;
                joint.damper = damper;
                joint.massScale = massScale;
            }
            else
            {
                // 🔥 Mostrar efecto de fallo en el punto de impacto
                if (hookFailEffectPrefab != null)
                {
                    GameObject effect = Instantiate(hookFailEffectPrefab, realHitPoint, Quaternion.LookRotation(-cam.forward));
                    Destroy(effect, 2f);
                }

                Invoke(nameof(StopSwing), 0.2f);

            }
        }
        else
        {
            // No golpeó nada, dispara al vacío
            realHitPoint = cam.position + cam.forward * maxSwingDistance;
            isAttemptingSwing = true;

            if (hookFailEffectPrefab != null)
            {
                Instantiate(hookFailEffectPrefab, realHitPoint, Quaternion.LookRotation(-cam.forward));
            }

            Invoke(nameof(StopSwing), 0.05f);  // También mostrar brevemente
        }
    }



    public void StopSwing()
    {
        isAttemptingSwing = false;
        realHitPoint = Vector3.zero;
        pm.swinging = false;

        if (lr != null)
        {
            lr.positionCount = 0;

            if (lr.gameObject.name == "GrappleLine")
            {
                Destroy(lr.gameObject);
                lr = null;
            }
        }

        if (joint != null)
        {
            Destroy(joint);
        }
    }

    private void OdmGearMovement()
    {
        if (Input.GetKey(KeyCode.D))
            rb.AddForce(transform.right * horizontalThrustForce * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            rb.AddForce(-transform.right * horizontalThrustForce * Time.deltaTime);
        if (Input.GetKey(KeyCode.W))
            rb.AddForce(transform.forward * horizontalThrustForce * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 dir = swingPoint - transform.position;
            rb.AddForce(dir.normalized * forwardThrustForce * Time.deltaTime);

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
        if (lr == null)
        {
            GameObject lrObj = new GameObject("GrappleLine");
            lr = lrObj.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.numCapVertices = 8;
        }

        lr.positionCount = 2;
    }

    private void UpdateLineToMouseDirection()
    {
        if (lr == null) return;
        realHitPoint = cam.position + cam.forward * maxSwingDistance;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, realHitPoint);
    }
}
