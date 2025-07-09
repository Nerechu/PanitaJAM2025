using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // this function used to Add button on MeshCombiner Scripts inspector.
        MeshCombiner MC = target as MeshCombiner;
        if (GUILayout.Button("Combine Selected", GUILayout.Width(250), GUILayout.Height(50)))
        {
            CombineSelected(MC);
        }
    }

    private void CombineSelected(MeshCombiner mC)
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        GameObject G = new GameObject("Combiner");
        G.transform.parent = selectedObjects[0].transform;
        G.transform.localPosition = Vector3.zero;
        G.transform.parent = selectedObjects[0].transform.parent;
        for (int i = 0; i < selectedObjects.Length; i++)
        {
            selectedObjects[i].transform.parent = G.transform;
        }
        G.AddComponent<MeshCombiner>();
        G.GetComponent<MeshCombiner>().CombineAllMeshes() ;
    }
}
