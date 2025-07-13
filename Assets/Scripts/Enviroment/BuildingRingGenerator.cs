using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class BuildingRingGenerator : MonoBehaviour
{
    [Header("Prefabs y Distribución")]
    public GameObject[] buildingPrefabs;
    public float density = 0.5f;

    [Header("Anillo")]
    public float innerRadius = 45f;
    public float outerRadius = 55f;

    [Header("Variación")]
    public Vector2 heightRange = new Vector2(0f, 10f);
    public Vector2 scaleRange = new Vector2(0.8f, 1.5f);

    [Header("Separación")]
    public float minDistanceBetweenBuildings = 10f;

    [Header("Ajustes Avanzados")]
    public bool rotateTowardsCenter = true;
    public bool removeCollidersAfterGeneration = true;
    public Transform centerPoint;

    [Header("Colisión")]
    public LayerMask collisionMask = ~0;
    public float boundsPadding = 1f;
    public int maxAttemptsPerBuilding = 10;

    [ContextMenu("Generar Anillo de Edificios")]
    public void GenerateBuildings()
    {
        if (buildingPrefabs == null || buildingPrefabs.Length == 0)
        {
            Debug.LogError("No hay prefabs asignados.");
            return;
        }

        if (centerPoint == null)
            centerPoint = this.transform;

        ClearChildren();

        float meanRadius = (innerRadius + outerRadius) / 2f;
        float circumference = 2f * Mathf.PI * meanRadius;
        int numberOfBuildings = Mathf.RoundToInt(circumference * density);

        List<GameObject> placedBuildings = new List<GameObject>();

        for (int i = 0; i < numberOfBuildings; i++)
        {
            bool placed = false;

            for (int attempt = 0; attempt < maxAttemptsPerBuilding; attempt++)
            {
                GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float radius = Random.Range(innerRadius, outerRadius);
                Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                Vector3 position = centerPoint.position + direction * radius;
                position.y += Random.Range(heightRange.x, heightRange.y);
                float scale = Random.Range(scaleRange.x, scaleRange.y);

                GameObject building = (GameObject)PrefabUtility.InstantiatePrefab(prefab, this.transform);
                building.transform.position = position;
                building.transform.localScale = Vector3.one * scale;

                if (rotateTowardsCenter)
                {
                    Vector3 lookDir = (centerPoint.position - position).normalized;
                    building.transform.rotation = Quaternion.LookRotation(lookDir);
                }

                Physics.SyncTransforms(); // importante para overlap

                Bounds bounds = CalculateTotalBounds(building);
                bounds.Expand(boundsPadding);

                bool overlaps = Physics.OverlapBox(bounds.center, bounds.extents / 2f, Quaternion.identity, collisionMask).Length > 1;

                bool tooClose = false;
                foreach (GameObject other in placedBuildings)
                {
                    if (Vector3.Distance(building.transform.position, other.transform.position) < minDistanceBetweenBuildings)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (overlaps || tooClose)
                {
#if UNITY_EDITOR
                    Debug.Log($"🔁 Reintentando: {(overlaps ? "colisión física" : "distancia")} en intento {attempt + 1}");
#endif
                    DestroyImmediate(building);
                    continue;
                }

                placedBuildings.Add(building);
                placed = true;
                break;
            }

            if (!placed)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"❌ No se pudo colocar edificio #{i} tras {maxAttemptsPerBuilding} intentos.");
#endif
            }
        }

        // ✅ Eliminar colliders
        if (removeCollidersAfterGeneration)
        {
            foreach (var go in placedBuildings)
            {
                foreach (var col in go.GetComponentsInChildren<Collider>())
                {
#if UNITY_EDITOR
                    DestroyImmediate(col);
#else
                    Destroy(col);
#endif
                }
            }
        }
    }

    private Bounds CalculateTotalBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds;
    }

    public void ClearChildren()
    {
#if UNITY_EDITOR
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
#endif
    }
}
