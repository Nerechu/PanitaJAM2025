using System.Collections.Generic;
using UnityEngine;

public class MeshManager : Singleton<MeshManager>
{
    Dictionary<string, MeshGroupRenderer> meshGroupRenderers;
    GameObject meshParent;

    public void addMesh(Transform t, Mesh mesh, Material material)
    {
        if (meshParent == null)
        {
            meshParent = new GameObject("meshParent");
        }

        if (meshGroupRenderers == null)
        {
            meshGroupRenderers = new Dictionary<string, MeshGroupRenderer>();
        }

        if (meshGroupRenderers.ContainsKey(material.name))
        {
            meshGroupRenderers[material.name].add(t, mesh, material);
        }
        else
        {
            GameObject render = new GameObject("meshGroup - " + material.name);
            render.transform.SetParent(meshParent.transform);

            MeshFilter mFilter = render.AddComponent<MeshFilter>();
            MeshRenderer mRenderer = render.AddComponent<MeshRenderer>();

            MeshGroupRenderer groupRenderer = render.AddComponent<MeshGroupRenderer>();
            groupRenderer.meshFilter = mFilter;
            groupRenderer.meshRenderer = mRenderer;
            groupRenderer.add(t, mesh, material);
            meshGroupRenderers.Add(material.name, groupRenderer);
        }
    }

    public void combineAll()
    {
        if (meshGroupRenderers != null)
        {
            foreach (var group in meshGroupRenderers)
            {
                group.Value.combineAndRender();
            }
            meshGroupRenderers.Clear();
            Resources.UnloadUnusedAssets();
        }
    }

    // Nuevo método para combinar solo los meshes bajo un Transform específico
    public void combineUnder(Transform parent)
    {
        if (meshParent == null)
        {
            meshParent = new GameObject("meshParent");
        }
        if (meshGroupRenderers == null)
        {
            meshGroupRenderers = new Dictionary<string, MeshGroupRenderer>();
        }

        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf == null || mf.sharedMesh == null) continue;
            Renderer rend = mf.GetComponent<Renderer>();
            if (rend == null || rend.sharedMaterial == null) continue;

            Material mat = rend.sharedMaterial;
            string matName = mat.name;

            if (meshGroupRenderers.ContainsKey(matName))
            {
                meshGroupRenderers[matName].add(mf.transform, mf.sharedMesh, mat);
            }
            else
            {
                GameObject render = new GameObject("meshGroup - " + matName);
                render.transform.SetParent(meshParent.transform);

                MeshFilter mFilter = render.AddComponent<MeshFilter>();
                MeshRenderer mRenderer = render.AddComponent<MeshRenderer>();

                MeshGroupRenderer groupRenderer = render.AddComponent<MeshGroupRenderer>();
                groupRenderer.meshFilter = mFilter;
                groupRenderer.meshRenderer = mRenderer;
                groupRenderer.add(mf.transform, mf.sharedMesh, mat);
                meshGroupRenderers.Add(matName, groupRenderer);
            }
        }

        // Combina los grupos generados
        foreach (var group in meshGroupRenderers)
        {
            group.Value.combineAndRender();
        }

        meshGroupRenderers.Clear();
        Resources.UnloadUnusedAssets();
    }
}
