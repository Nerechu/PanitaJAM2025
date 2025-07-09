using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshCombiner : MonoBehaviour
{

    public void CombineAllMeshes()
    {
        //This Function Will merge meshes of the children and Deactivate children.
#if UNITY_EDITOR
        Material mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/CityBuilderTool/mat/VertexLit.mat", typeof(Material));
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = mat;

        Vector3 oldPos = transform.position;
        Quaternion oldrot = transform.rotation;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        MeshFilter[] allMeshFilters = GetComponentsInChildren<MeshFilter>();
        Mesh finalMesh = new Mesh();
        finalMesh.name = "Mesh" + Mathf.Abs((int)gameObject.GetInstanceID());
        CombineInstance[] CombineInstanc = new CombineInstance[allMeshFilters.Length];
        for (int i = 0; i < allMeshFilters.Length; i++)
        {
            CombineInstanc[i].subMeshIndex = 0;
            CombineInstanc[i].mesh = allMeshFilters[i].sharedMesh;
            CombineInstanc[i].transform = allMeshFilters[i].transform.localToWorldMatrix;
        }
        finalMesh.CombineMeshes(CombineInstanc);
        GetComponent<MeshFilter>().sharedMesh = finalMesh;

        transform.position = oldPos;
        transform.rotation = oldrot;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag != "Colliders")
            {
                if (transform.GetChild(i).tag != "Floor")
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    for (int j = 0; j < transform.GetChild(i).childCount; j++)
                    {
                        for (int k = 0; k < transform.GetChild(i).GetChild(j).childCount; k++)
                        {
                            if (transform.GetChild(i).GetChild(j).GetChild(k).gameObject.tag != "Colliders")
                            {
                                transform.GetChild(i).GetChild(j).GetChild(k).gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
        }

#endif
    }
    public void RevertCombination()
    {
        //This Function Will revert Mesh merging and re-activate children.
#if UNITY_EDITOR
        MeshFilter[] allMeshFilters = GetComponentsInChildren<MeshFilter>();
        Mesh finalMesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = finalMesh;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            for (int j = 0; j < transform.GetChild(i).childCount; j++)
            {
                for (int k = 0; k < transform.GetChild(i).GetChild(j).childCount; k++)
                {
                    if (transform.GetChild(i).GetChild(j).GetChild(k).gameObject.tag != "Colliders")
                    {
                        transform.GetChild(i).GetChild(j).GetChild(k).gameObject.SetActive(true);
                    }
                }
            }
        }
#endif
    }
}