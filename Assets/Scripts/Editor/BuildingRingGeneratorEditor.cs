using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingRingGenerator))]
public class BuildingRingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BuildingRingGenerator generator = (BuildingRingGenerator)target;

        GUILayout.Space(10);

        if (GUILayout.Button("➕ Generar Anillo de Edificios"))
        {
            generator.GenerateBuildings();
        }

        if (GUILayout.Button("🧹 Limpiar Anillo Actual"))
        {
            generator.ClearChildren();
        }
    }
}
