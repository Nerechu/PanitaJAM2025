using UnityEngine;

public class FpsLook : MonoBehaviour
{
    [Header("Settings")]
    public float sensitivity = 35f;
    public float smoothTime = 5f;

    [Header("References")]
    public Transform playerBody;

    private float xRotation = 0f;
    private Vector2 currentMouseDelta;
    private Vector2 smoothMouseDelta;
    private Vector2 mouseDeltaVelocity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerBody == null)
            Debug.LogError("Asigna el Player al campo 'playerBody'");
    }

    private void Update()
    {
        Vector2 rawMouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        smoothMouseDelta = Vector2.SmoothDamp(smoothMouseDelta, rawMouse, ref mouseDeltaVelocity, 1f / smoothTime);

        xRotation -= smoothMouseDelta.y * sensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Aplica rotación vertical a la cámara
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Aplica rotación horizontal al cuerpo del jugador
        playerBody.Rotate(Vector3.up * smoothMouseDelta.x * sensitivity * Time.deltaTime);
    }
}
