using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using static piecesNamingScript;

[InitializeOnLoad]
public class CityBuilderTool : Editor
{
    static GameObject TheMap, StreetParent;
    static GameObject SpawnBuilding;
    static CityScript CitySc;
    static float zpos, xpos;
    public static bool ToolIsOn;
    public static TextureManager TM;
    public static piecesNamingScript Naming = new piecesNamingScript();
    static float ButtonHeight = 50;
    static string CurrentUsedObject;
    [MenuItem("Window/Low Poly City Builder")]
    public static void Enable()
    {
        // Create Texturemanager -then setup textures for the buttons.
        TM = new TextureManager();
        TM.SetupTextureAndGuiStyles();

        // This function makes sure There is a Tag called "Map".
        Addtag("Map");

        //Create the tool one time
        if (!ToolIsOn)
        {
            SceneView.duringSceneGui += OnSceneGUI;
            GetOrCreateMap();
            ToolIsOn = true;
        }
    }

    private static void GetOrCreateMap()
    {
        //This function will makes sure There is  a map gameobject with tag "Map"

        TheMap = GameObject.FindGameObjectWithTag("Map");
        if (TheMap == null)
        {
            TheMap = new GameObject("Map");
            TheMap.tag = "Map";
            TheMap.name = "Map" + Mathf.Abs((int)TheMap.GetInstanceID());
            TheMap.AddComponent<CityScript>();
            CitySc = TheMap.GetComponent<CityScript>();
            TheMap.AddComponent<CityMeshCombiner>();
        }
        else
        {
            CitySc = TheMap.GetComponent<CityScript>();
        }

        StreetParent = GameObject.FindGameObjectWithTag("StreetParent");
        if (StreetParent == null)
        {
            StreetParent = new GameObject("StreetParent");
            StreetParent.transform.parent = TheMap.transform;
            Addtag("StreetParent");
            StreetParent.tag = "StreetParent";
        }
    }

    static void OnSceneGUI(SceneView sceneview)
    {
        SceneMouse();
        SceneButtons(sceneview);
    }

    private static void SceneMouse()
    {
        //here the mouse functions.

        if (SpawnBuilding != null)
        {
            SpawnBuilding.transform.position = MouseWorldPositionOnGrid(6);
        }
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.shift)
        {
            if (SpawnBuilding != null)
            {
                Vector3 SlotPos = MouseWorldPositionOnGrid(6);
                int Column = (int)(SlotPos.x / 6);
                int Row = (int)(SlotPos.z / 6);
                if (CitySc.CheckEmptyCell(Column, Row))
                {
                    if (SpawnBuilding.GetComponent<PavementScript>() != null)
                    {
                        SpawnBuilding.GetComponent<PavementScript>().SetRowAndColumns();
                        CitySc.ReCheckBuildings(Column, Row);
                        SpawnBuilding = null;
                        SpawnBuilding = Instantiate(Resources.Load(CurrentUsedObject)) as GameObject;
                        SpawnBuilding.transform.parent = TheMap.transform;
                    }
                    else if (SpawnBuilding.GetComponent<StreetScript>() != null)
                    {
                        SpawnBuilding.GetComponent<StreetScript>().SetRowAndColumns(Column, Row);
                        CitySc.ReCheckAndBuild_Streets(Column, Row);
                        SpawnBuilding = null;
                        SpawnBuilding = Instantiate(Resources.Load(CurrentUsedObject)) as GameObject;
                        SpawnBuilding.transform.parent = StreetParent.transform;
                    }
                }
            }
        }
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Event.current.shift)
        {
            DestroyImmediate(SpawnBuilding);
            SpawnBuilding = null;
        }
    }

    private static void SceneButtons(SceneView sceneview)
    {
        //here all the Buttons and Its functions.
        Handles.BeginGUI();
        GUILayout.BeginVertical();
        GUILayout.Label("", GUILayout.Width(0), GUILayout.Height(sceneview.camera.pixelHeight - 400));
        if (GUILayout.Button(new GUIContent("", "Close"), TM.Style_Close, GUILayout.Width(ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            ToolIsOn = false;
            CurrentUsedObject = "";
            positioningOn6by6gird();
        }
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(new GUIContent("", "Create New Building"), TM.Style_NewBuilding, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            if (SpawnBuilding != null)
            {
                DestroyImmediate(SpawnBuilding);
                SpawnBuilding = null;
            }
            SpawnBuilding = Instantiate(Resources.Load("building")) as GameObject;
            CurrentUsedObject = "building";
            SpawnBuilding.transform.position = Vector3.zero;
            try
            {
                SpawnBuilding.transform.parent = TheMap.transform;
            }
            catch (Exception e)
            {
                GetOrCreateMap();
                SpawnBuilding.transform.parent = TheMap.transform;
            }
        }


        if (GUILayout.Button(new GUIContent("", "Add Floors to selected"), TM.Style_AddFloor, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length > 0)
            {
                List<GameObject> GList = GetListOfBuildingInSelection(selectedObjects);

                for (int i = 0; i < GList.Count; i++)
                {
                    GList[i].GetComponent<FloorsEditor>().FloorsCount++;
                    GList[i].GetComponent<FloorsEditor>().UpdateFloors();
                }
            }
            Selection.objects = selectedObjects;
        }
        if (GUILayout.Button(new GUIContent("", "Remove Floors from selected"), TM.Style_RemoveFloor, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length > 0)
            {
                List<GameObject> GList = GetListOfBuildingInSelection(selectedObjects);
                for (int i = 0; i < GList.Count; i++)
                {
                    if (GList[i].GetComponent<FloorsEditor>().FloorsCount > 1)
                    {
                        GList[i].GetComponent<FloorsEditor>().FloorsCount--;
                        GList[i].GetComponent<FloorsEditor>().UpdateFloors();
                    }
                }
            }

            Selection.objects = selectedObjects;
        }


        if (GUILayout.Button(new GUIContent("", "Merge mesh"), TM.Style_MergeMesh, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            TheMap.GetComponent<CityMeshCombiner>().CombineCity();
        }
        if (GUILayout.Button(new GUIContent("", "Revert Mesh Merge"), TM.Style_RevertMeshMerge, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            TheMap.GetComponent<CityMeshCombiner>().RevertCity();
        }
        if (GUILayout.Button(new GUIContent("", "Delete Selected Buildings"), TM.Style_Building_delete, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length > 0)
            {
                List<GameObject> GList = GetListOfBuildingInSelection(selectedObjects);
                for (int i = 0; i < GList.Count; i++)
                {
                    CitySc.RemoveAndUpdateBuildings(GList[i].GetComponent<PavementScript>().Column, GList[i].GetComponent<PavementScript>().Row);
                }
            }

            Selection.objects = selectedObjects;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("", TM.StyleStreet_Empty, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            if (SpawnBuilding != null)
            {
                DestroyImmediate(SpawnBuilding);
                SpawnBuilding = null;
            }
            SpawnBuilding = Instantiate(Resources.Load("Street_Empty")) as GameObject;
            CurrentUsedObject = "Street_Empty";
            SpawnBuilding.transform.position = Vector3.zero;
            SpawnBuilding.transform.parent = StreetParent.transform;
        }

        if (GUILayout.Button("", TM.StyleStreet_Straight, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            if (SpawnBuilding != null)
            {
                DestroyImmediate(SpawnBuilding);
                SpawnBuilding = null;
            }
            SpawnBuilding = Instantiate(Resources.Load("Street_Straight")) as GameObject;
            CurrentUsedObject = "Street_Straight";
            SpawnBuilding.transform.position = Vector3.zero;
            SpawnBuilding.transform.parent = StreetParent.transform;
        }
        if (GUILayout.Button("", TM.StyleStreet_Turn, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            if (SpawnBuilding != null)
            {
                DestroyImmediate(SpawnBuilding);
                SpawnBuilding = null;
            }
            SpawnBuilding = Instantiate(Resources.Load("Street_Turn")) as GameObject;
            CurrentUsedObject = "Street_Turn";
            SpawnBuilding.transform.position = Vector3.zero;
            SpawnBuilding.transform.parent = StreetParent.transform;
        }
        if (GUILayout.Button("", TM.StyleStreet_CrossLine, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            if (SpawnBuilding != null)
            {
                DestroyImmediate(SpawnBuilding);
                SpawnBuilding = null;
            }
            SpawnBuilding = Instantiate(Resources.Load("Street_CrossLine")) as GameObject;
            CurrentUsedObject = "Street_CrossLine";
            SpawnBuilding.transform.position = Vector3.zero;
            SpawnBuilding.transform.parent = StreetParent.transform;
        }
        if (GUILayout.Button(new GUIContent("", "Rotate Selected street"), TM.Style_Street_Rotate, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length > 0)
            {
                for (int i = 0; i < selectedObjects.Length; i++)
                {
                    if (selectedObjects[i].GetComponent<StreetScript>() != null)
                    {
                        selectedObjects[i].transform.Rotate(0, 90, 0);
                    }
                }
            }
        }
        if (GUILayout.Button(new GUIContent("", "Delete Selected Street"), TM.Style_Street_Delete, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length > 0)
            {
                for (int i = 0; i < selectedObjects.Length; i++)
                {

                    try

                    {
                        if (selectedObjects[i].GetComponent<StreetScript>() != null)
                        {
                            CitySc.RemoveAndUpdateStreet(selectedObjects[i].GetComponent<StreetScript>().col, selectedObjects[i].GetComponent<StreetScript>().row);
                        }
                    }
                    catch (Exception e) { }
                }
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(5);


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("", TM.StyleBlue, GUILayout.Width(1.714f * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceColorWithTheSameMesh(ColorName.Cyan);
        }
        if (GUILayout.Button("", TM.StyleBlueGreen, GUILayout.Width(1.714f * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceColorWithTheSameMesh(ColorName.Bluo);

        }
        if (GUILayout.Button("", TM.StyleGreen, GUILayout.Width(1.714f * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceColorWithTheSameMesh(ColorName.Green);

        }
        if (GUILayout.Button("", TM.StyleGrey, GUILayout.Width(1.714f * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceColorWithTheSameMesh(ColorName.Grey);

        }
        if (GUILayout.Button("", TM.StyleOrange, GUILayout.Width(1.714f * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceColorWithTheSameMesh(ColorName.Orange);

        }
        if (GUILayout.Button("", TM.StyleRed, GUILayout.Width(1.714f * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceColorWithTheSameMesh(ColorName.Red);
        }
        if (GUILayout.Button("", TM.StyleRose, GUILayout.Width(1.714f * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceColorWithTheSameMesh(ColorName.Rose);

        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("", TM.Style_threeNarrowWindows, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_threeNarrowWindows_);
        }
        if (GUILayout.Button("", TM.Style_three_Windows, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_three_Windows_);

        }
        if (GUILayout.Button("", TM.Style_Balcony, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_Balcony_);

        }
        if (GUILayout.Button("", TM.Style_Balcony01, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_Balcony01_);

        }
        if (GUILayout.Button("", TM.Style_Balcony02, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_Balcony02_);

        }
        if (GUILayout.Button("", TM.Style_Balcony03, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_Balcony03_);

        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("", TM.Style_shop1, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_shop1_);

        }
        if (GUILayout.Button("", TM.Style_shop2, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_shop2_);

        }
        if (GUILayout.Button("", TM.Style_shop3, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_shop3_);

        }
        if (GUILayout.Button("", TM.Style_shop4, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_shop4_);

        }
        if (GUILayout.Button("", TM.Style_shop5, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_shop5_);

        }
        if (GUILayout.Button("", TM.Style_shop6, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_shop6_);

        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("", TM.Style_Empty, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_Empty_);

        }
        if (GUILayout.Button("", TM.Style_Empty_AC, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_Empty_AC_);

        }
        if (GUILayout.Button("", TM.Style_LWindowSide, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_LWindowSide_);

        }
        if (GUILayout.Button("", TM.Style_garage, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_garage_);

        }
        if (GUILayout.Button("", TM.Style_Door_Entrance, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_Door_Entrance_);

        }
        if (GUILayout.Button("", TM.Style_sideAlleyEntrance, GUILayout.Width(2 * ButtonHeight), GUILayout.Height(ButtonHeight)))
        {
            ReplaceMeshWithTheSameColor(BuildingPiece.CM_Wallside_sideAlleyEntrance_);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        Handles.EndGUI();

    }

    private static void positioningOn6by6gird()
    {
        //This function makes sure the mouse locks on points on grid 6 by 6
        if (TheMap != null)
        {
            for (int i = 0; i < TheMap.transform.childCount; i++)
            {
                TheMap.transform.GetChild(i).localPosition = new Vector3(Mathf.Round(TheMap.transform.GetChild(i).localPosition.x / 6) * 6, TheMap.transform.GetChild(i).localPosition.y, Mathf.Round(TheMap.transform.GetChild(i).localPosition.z / 6) * 6);
            }
        }
    }

    private static void ReplaceColorWithTheSameMesh(ColorName col)
    {
        //This Function replace facades with another one with the same type;
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length > 0)
        {
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                if (selectedObjects[i].GetComponent<Piecedata>() != null)
                {
                    GameObject G = ReplaceWith(selectedObjects[i], Naming.getPieceName(selectedObjects[i].GetComponent<Piecedata>().buildingpiece, col));
                    selectedObjects[i] = G;
                }
            }
        }
        Selection.objects = selectedObjects;
    }

    private static void ReplaceMeshWithTheSameColor(BuildingPiece Piece)
    {
        //This Function replace facades with another one with the same Color;
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length > 0)
        {
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                if (selectedObjects[i].GetComponent<Piecedata>() != null)
                {
                    if (selectedObjects[i].GetComponent<Piecedata>().buildingpiece == BuildingPiece.CM_Top_Ring_)
                    {

                    }
                    else
                    {
                        GameObject G = ReplaceWith(selectedObjects[i], Naming.getPieceName(Piece, selectedObjects[i].GetComponent<Piecedata>().colorname));
                        selectedObjects[i] = G;
                    }
                }
            }
        }
        Selection.objects = selectedObjects;
    }

    private static GameObject ReplaceWith(GameObject SelectedgameObject, string NewGameObjectName)
    {
        //This Function replace A gameobject with another one with the same position and rotation-(use this function to Replace the facades)
        GameObject Piece = Instantiate(Resources.Load(NewGameObjectName)) as GameObject;
        Piece.transform.parent = SelectedgameObject.transform;
        Piece.transform.localPosition = Vector3.zero;
        Piece.transform.localRotation = Quaternion.identity;
        Piece.transform.parent = SelectedgameObject.transform.parent;
        Piece.transform.localScale = Vector3.one;
        DestroyImmediate(SelectedgameObject);
        return Piece;
    }

    private static List<GameObject> GetListOfBuildingInSelection(GameObject[] selectedObjects)
    {
        //This function Return a list of all the Buildings (if any part of it selected).
        List<GameObject> BE = new List<GameObject>();
        for (int i = 0; i < selectedObjects.Length; i++)
        {
            try
            {
                if (selectedObjects[i].GetComponent<PavementScript>() != null)
                {
                    if (!BE.Contains(selectedObjects[i].GetComponent<GameObject>()))
                    {
                        BE.Add(selectedObjects[i]);
                    }
                }

                if (selectedObjects[i].transform.parent.GetComponent<PavementScript>() != null)
                {
                    if (!BE.Contains(selectedObjects[i].transform.parent.gameObject))
                    {
                        BE.Add(selectedObjects[i].transform.parent.gameObject);
                    }
                }

                if (selectedObjects[i].transform.parent.parent.GetComponent<PavementScript>() != null)
                {
                    if (!BE.Contains(selectedObjects[i].transform.parent.parent.gameObject))
                    {
                        BE.Add(selectedObjects[i].transform.parent.parent.gameObject);
                    }
                }
                if (selectedObjects[i].transform.parent.parent.parent.GetComponent<PavementScript>() != null)
                {
                    if (!BE.Contains(selectedObjects[i].transform.parent.parent.parent.gameObject))
                    {
                        BE.Add(selectedObjects[i].transform.parent.parent.parent.gameObject);
                    }
                }
            }
            catch (Exception e) { }
        }
        return BE;
    }


    public static Vector3 MouseWorldPositionOnGrid(float SpaceLength)
    {
        //This function return the Exact (position) where a raycast from the Mouse Hits the Floor plane

        Vector3 mouseWorld = Event.current.mousePosition;
        mouseWorld.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mouseWorld.y;
        Ray ray = SceneView.currentDrawingSceneView.camera.ScreenPointToRay(mouseWorld);
        Plane hPlane = new Plane(Vector3.up, Vector3.zero);
        float distance = 0;
        if (hPlane.Raycast(ray, out distance))
        {
            mouseWorld = ray.GetPoint(distance);
        }
        if (mouseWorld.x > 0)
        {
            xpos = (int)((mouseWorld.x + SpaceLength / 2) / SpaceLength) * SpaceLength;
        }
        else
        {
            xpos = (int)((mouseWorld.x - SpaceLength / 2) / SpaceLength) * SpaceLength;
        }
        if (mouseWorld.z > 0)
        {
            zpos = (int)((mouseWorld.z + SpaceLength / 2) / SpaceLength) * SpaceLength;

        }
        else
        {
            zpos = (int)((mouseWorld.z - SpaceLength / 2) / SpaceLength) * SpaceLength;

        }
        mouseWorld = new Vector3(xpos, 0, zpos);
        return mouseWorld;
    }

    private static void Addtag(string tagname)
    {
        // add tag if not existing.
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if ((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            for (int i = 0; i < tags.arraySize; ++i)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == tagname)
                {
                    return;
                }
            }
            tags.InsertArrayElementAtIndex(0);
            tags.GetArrayElementAtIndex(0).stringValue = tagname;
            so.ApplyModifiedProperties();
            so.Update();
        }
    }
}