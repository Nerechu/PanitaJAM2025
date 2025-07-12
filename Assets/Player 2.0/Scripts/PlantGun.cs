using UnityEngine;

public class PlantGun : MonoBehaviour
{
    [Header("Disparo")]
    public Camera fpsCamera;
    public Transform shotOrigin;
    public ProceduralIvy ivySystem;
    public LayerMask plantLayer;
    public float launchForce = 80f;

    [Header("Semilla")]
    public GameObject seedPrefab;

    [Header("Feedback visual")]
    public GameObject impactParticlePrefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireSeed();
        }
    }

    void FireSeed()
    {
        if (seedPrefab == null) return;

        // Crear la semilla en la posición y dirección de la cámara
        GameObject seed = Instantiate(seedPrefab, shotOrigin.position, Quaternion.LookRotation(fpsCamera.transform.forward));

        // Obtener Rigidbody y aplicar fuerza
        Rigidbody rb = seed.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = fpsCamera.transform.forward * launchForce;
        }

        // Asignar variables necesarias
        SeedProjectile seedScript = seed.GetComponent<SeedProjectile>();
        if (seedScript != null)
        {
            seedScript.impactParticlePrefab = impactParticlePrefab;
            seedScript.ivySystem = ivySystem;
            seedScript.plantLayer = plantLayer;
        }

        // Ignorar colisión con todos los colliders del jugador (arma + cuerpo)
        Collider seedCollider = seed.GetComponent<Collider>();
        Collider[] ownColliders = GetComponentsInParent<Collider>();
        if (seedCollider != null)
        {
            foreach (Collider col in ownColliders)
            {
                Physics.IgnoreCollision(seedCollider, col);
            }
        }

        // Debug opcional (visible en escena)
        Debug.DrawRay(shotOrigin.position, fpsCamera.transform.forward * 10, Color.red, 2f);
    }
}
