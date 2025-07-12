using UnityEngine;
using System.Collections; // ← Necesario para IEnumerator (corutinas)

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

    [Header("Animación de Retroceso")]
    public Transform gunModel;
    public Vector3 recoilOffset = new Vector3(0, -0.1f, -0.2f);
    private Vector3 originalGunPosition;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootClip;

    void Awake()
    {
        if (gunModel != null)
            originalGunPosition = gunModel.localPosition;
    }

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

        // Reproducir animación de retroceso
        if (gunModel != null)
        {
            StopAllCoroutines();
            StartCoroutine(PlayRecoil());
        }

        // Reproducir sonido de disparo
        if (audioSource != null && shootClip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(shootClip);
        }

        // Debug opcional (visible en escena)
        Debug.DrawRay(shotOrigin.position, fpsCamera.transform.forward * 10, Color.red, 2f);
    }

    private IEnumerator PlayRecoil()
    {
        Vector3 target = originalGunPosition + recoilOffset;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            gunModel.localPosition = Vector3.Lerp(originalGunPosition, target, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            gunModel.localPosition = Vector3.Lerp(target, originalGunPosition, t);
            yield return null;
        }

        gunModel.localPosition = originalGunPosition;
    }
}
