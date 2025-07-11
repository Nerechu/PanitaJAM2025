using UnityEngine;

public class SeedProjectile : MonoBehaviour
{
    public GameObject impactParticlePrefab;
    public ProceduralIvy ivySystem;
    public LayerMask plantLayer;

    private bool hasImpacted = false;

    private void Start()
    {
        // Destruye el proyectil autom�ticamente despu�s de 2 segundos si no impacta
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasImpacted) return;
        hasImpacted = true;

        ContactPoint contact = collision.contacts[0];

        // Instanciar part�culas de impacto
        if (impactParticlePrefab != null)
        {
            GameObject impact = Instantiate(impactParticlePrefab, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(impact, 2f);
        }

        // Verificar si impact� contra capa vegetal
        if (((1 << collision.gameObject.layer) & plantLayer) != 0)
        {
            ivySystem.createIvy(contact.point, contact.normal);
            ivySystem.combineAndClear();

            //Audio

            AudioManager.instance.PlaySound(SoundType.SEEDPLANTED, .6f);
            AudioManager.instance.PlaySound(SoundType.PLANTGROWTH);
        }

        else { AudioManager.instance.PlaySound(SoundType.SEEDMISSED); }

        Destroy(gameObject);
    }
}
