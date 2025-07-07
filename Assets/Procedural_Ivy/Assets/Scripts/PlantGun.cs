using UnityEngine;
using System.Collections;

public class PlantGun : MonoBehaviour
{
    [Header("Disparo")]
    public Camera fpsCamera;
    public Transform shotOrigin;
    public ProceduralIvy ivySystem;
    public LayerMask plantLayer;
    public float maxDistance = 100f;

    [Header("Feedback Visual")]
    public LineRenderer shotLine;
    public float shotDuration = 0.05f;

    [Header("Impacto")]
    public GameObject impactParticlePrefab; // ← NUEVO: prefab de partícula de impacto

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(fpsCamera.transform.position, fpsCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                // Línea del disparo
                StartCoroutine(ShowShotRay(shotOrigin.position, hit.point));

                // Efecto de impacto visual
                SpawnImpactParticle(hit.point, hit.normal);

                // Interacción con capa de plantas
                if (((1 << hit.collider.gameObject.layer) & plantLayer) != 0)
                {
                    ivySystem.createIvy(hit);
                    ivySystem.combineAndClear();
                }
            }
            else
            {
                // Disparo al aire
                StartCoroutine(ShowShotRay(shotOrigin.position, shotOrigin.position + fpsCamera.transform.forward * maxDistance));
            }
        }
    }

    void SpawnImpactParticle(Vector3 position, Vector3 normal)
    {
        if (impactParticlePrefab != null)
        {
            GameObject impact = Instantiate(impactParticlePrefab, position, Quaternion.LookRotation(normal));
            Destroy(impact, 2f); // Destruir después de 2 segundos
        }
    }

    IEnumerator ShowShotRay(Vector3 start, Vector3 end)
    {
        if (shotLine == null)
            yield break;

        shotLine.SetPosition(0, start);
        shotLine.SetPosition(1, end);
        shotLine.enabled = true;

        yield return new WaitForSeconds(shotDuration);

        shotLine.enabled = false;
    }
}
