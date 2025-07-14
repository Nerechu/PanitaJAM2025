using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class SeedProjectile : MonoBehaviour
{
    public GameObject impactParticlePrefab;
    public ProceduralIvy ivySystem;
    public LayerMask plantLayer;

    private bool hasImpacted = false;

    private void Awake()
    {
        // Activar interpolación para suavizar el movimiento en movimiento rápido
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    private void Start()
    {
        // Destruir automáticamente si no impacta en 2 segundos
        Destroy(gameObject, 4f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasImpacted) return;
        hasImpacted = true;

        ContactPoint contact = collision.contacts[0];

        // Partículas de impacto
        if (impactParticlePrefab != null)
        {
            GameObject impact = Instantiate(impactParticlePrefab, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(impact, 2f);
        }

        // Si impacta con capa vegetal
        if (((1 << collision.gameObject.layer) & plantLayer) != 0)
        {
            ivySystem.createIvy(contact.point, contact.normal);
            ivySystem.combineAndClear();

            AudioManager.instance.PlaySound(SoundType.SEEDPLANTED);
            AudioManager.instance.PlaySound(SoundType.PLANTGROWTH);
        }
        else
        {
            AudioManager.instance.PlaySound(SoundType.SEEDMISSED);
        }

        Destroy(gameObject);
    }
}
