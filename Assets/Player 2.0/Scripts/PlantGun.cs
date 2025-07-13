using UnityEngine;
using System.Collections;

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

        // Raycast desde el centro del crosshair
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 100f;
        }

        // Calcular dirección desde el arma hacia el punto objetivo
        Vector3 shootDirection = (targetPoint - shotOrigin.position).normalized;

        // Instanciar semilla y orientarla
        GameObject seed = Instantiate(seedPrefab, shotOrigin.position, Quaternion.LookRotation(shootDirection));

        // Sonido (sistema personalizado)
        AudioManager.instance.PlaySound(SoundType.FIRESEED);

        // Aplicar velocidad
        Rigidbody rb = seed.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * launchForce;
        }

        // Asignar dependencias
        SeedProjectile seedScript = seed.GetComponent<SeedProjectile>();
        if (seedScript != null)
        {
            seedScript.impactParticlePrefab = impactParticlePrefab;
            seedScript.ivySystem = ivySystem;
            seedScript.plantLayer = plantLayer;
        }

        // Ignorar colisiones con el jugador y el arma
        Collider seedCollider = seed.GetComponent<Collider>();
        Collider[] ownColliders = GetComponentsInParent<Collider>();
        if (seedCollider != null)
        {
            foreach (Collider col in ownColliders)
            {
                Physics.IgnoreCollision(seedCollider, col);
            }
        }

        // Recoil visual
        if (gunModel != null)
        {
            StopAllCoroutines();
            StartCoroutine(PlayRecoil());
        }

        // Sonido con pitch aleatorio
        if (audioSource != null && shootClip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(shootClip);
        }

        // Debug visual en la escena
        Debug.DrawRay(shotOrigin.position, shootDirection * 100f, Color.red, 2f);
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
