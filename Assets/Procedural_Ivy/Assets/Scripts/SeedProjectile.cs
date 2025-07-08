using UnityEngine;

public class SeedProjectile : MonoBehaviour
{
    public GameObject impactParticlePrefab;
    public ProceduralIvy ivySystem;
    public LayerMask plantLayer;

    private bool hasImpacted = false;

    private void Start()
    {
        // Destruye el proyectil automáticamente después de 2 segundos si no impacta
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasImpacted) return;
        hasImpacted = true;

        ContactPoint contact = collision.contacts[0];

        // Instanciar partículas de impacto
        if (impactParticlePrefab != null)
        {
            GameObject impact = Instantiate(impactParticlePrefab, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(impact, 2f);
        }

        // Verificar si impactó contra capa vegetal
        if (((1 << collision.gameObject.layer) & plantLayer) != 0)
        {
            ivySystem.createIvy(contact.point, contact.normal);
            ivySystem.combineAndClear();
        }

        Destroy(gameObject);
    }
}
