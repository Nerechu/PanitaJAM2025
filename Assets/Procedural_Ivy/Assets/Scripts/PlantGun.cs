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
    public GameObject impactParticlePrefab;   // Partícula al impactar

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireSeed();
        }
    }

    void FireSeed()
    {
        if (seedPrefab != null)
        {
            GameObject seed = Instantiate(seedPrefab, shotOrigin.position, Quaternion.identity);

            Rigidbody rb = seed.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = fpsCamera.transform.forward * launchForce;
            }

            SeedProjectile seedScript = seed.GetComponent<SeedProjectile>();
            if (seedScript != null)
            {
                seedScript.impactParticlePrefab = impactParticlePrefab;
                seedScript.ivySystem = ivySystem;
                seedScript.plantLayer = plantLayer;
            }

            // Ignorar colisión entre el proyectil y este objeto (arma/jugador)
            Collider seedCollider = seed.GetComponent<Collider>();
            Collider ownCollider = GetComponent<Collider>();

            if (seedCollider != null && ownCollider != null)
            {
                Physics.IgnoreCollision(seedCollider, ownCollider);
            }
        }
    }
}
