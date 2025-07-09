using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    Texture2D Blue, BlueGreen, Green, Grey, Orange, Red, Rose, White;
    Texture2D threeNarrowWindows, three_Windows, sideAlleyEntrance, shop6, shop5, shop4, shop3, shop2, shop1, LWindowSide, garage, Empty, Empty_AC, Door_Entrance, Balcony, Balcony03, Balcony02, Balcony01;
    Texture2D Street_CrossLine, Street_Empty, Street_Straight, Street_Turn;
    public GUIStyle StyleBlue, StyleBlueGreen, StyleGreen, StyleGrey, StyleOrange, StyleRed, StyleRose, StyleWhite;
    public GUIStyle Style_threeNarrowWindows, Style_three_Windows, Style_sideAlleyEntrance, Style_shop6, Style_shop5, Style_shop4, Style_shop3, Style_shop2, Style_shop1, Style_LWindowSide, Style_garage, Style_Empty, Style_Empty_AC, Style_Door_Entrance, Style_Balcony, Style_Balcony03, Style_Balcony02, Style_Balcony01;
    public GUIStyle StyleStreet_CrossLine, StyleStreet_Empty, StyleStreet_Straight, StyleStreet_Turn;

    Texture2D AddFloor, RemoveFloor, Street_delete, MergeMesh, RevertMeshMerge, Building_delete, Street_Rotate, NewBuilding, CloseTex;
    public GUIStyle Style_AddFloor, Style_RemoveFloor, Style_Building_delete, Style_MergeMesh, Style_RevertMeshMerge, Style_Street_Delete, Style_Street_Rotate, Style_NewBuilding,Style_Close;


    public void SetupTextureAndGuiStyles()
    {
        //this function loads all the texture and setup guistyles To be used in UI buttons.

        Blue = Resources.Load("Tex/Blue") as Texture2D;
        BlueGreen = Resources.Load("Tex/BlueGreen") as Texture2D;
        Green = Resources.Load("Tex/Green") as Texture2D;
        Grey = Resources.Load("Tex/Grey") as Texture2D;
        Orange = Resources.Load("Tex/Orange") as Texture2D;
        Red = Resources.Load("Tex/Red") as Texture2D;
        Rose = Resources.Load("Tex/Rose") as Texture2D;
        White = Resources.Load("Tex/White") as Texture2D;

        threeNarrowWindows = Resources.Load("Tex/threeWindows") as Texture2D;
        three_Windows = Resources.Load("Tex/Window") as Texture2D;
        sideAlleyEntrance = Resources.Load("Tex/Alley") as Texture2D;
        shop6 = Resources.Load("Tex/shop6") as Texture2D;
        shop5 = Resources.Load("Tex/shop5") as Texture2D;
        shop4 = Resources.Load("Tex/shop4") as Texture2D;
        shop3 = Resources.Load("Tex/shop3") as Texture2D;
        shop2 = Resources.Load("Tex/shop2") as Texture2D;
        shop1 = Resources.Load("Tex/shop1") as Texture2D;
        LWindowSide = Resources.Load("Tex/Glass") as Texture2D;
        garage = Resources.Load("Tex/garage") as Texture2D;
        Empty = Resources.Load("Tex/Empty") as Texture2D;
        Empty_AC = Resources.Load("Tex/Empty_AC") as Texture2D;
        Door_Entrance = Resources.Load("Tex/Door") as Texture2D;
        Balcony = Resources.Load("Tex/Balcony") as Texture2D;
        Balcony03 = Resources.Load("Tex/Balcony03") as Texture2D;
        Balcony02 = Resources.Load("Tex/Balcony02") as Texture2D;
        Balcony01 = Resources.Load("Tex/Glass2") as Texture2D;

        AddFloor = Resources.Load("Tex/AddFloor") as Texture2D;
        RemoveFloor = Resources.Load("Tex/RemoveFloor") as Texture2D;
        MergeMesh = Resources.Load("Tex/MergeMesh") as Texture2D;
        RevertMeshMerge = Resources.Load("Tex/RevertMeshMerge") as Texture2D;

        Street_CrossLine = Resources.Load("Tex/Street_CrossLine") as Texture2D;
        Street_Empty = Resources.Load("Tex/Street_Empty") as Texture2D;
        Street_Straight = Resources.Load("Tex/Street_Straight") as Texture2D;
        Street_Turn = Resources.Load("Tex/Street_Turn") as Texture2D;
        Building_delete = Resources.Load("Tex/Building_delete") as Texture2D;
        Street_Rotate = Resources.Load("Tex/Street_Rotate") as Texture2D;
        Street_delete = Resources.Load("Tex/Street_delete") as Texture2D;

        NewBuilding = Resources.Load("Tex/NewBuilding") as Texture2D;
        CloseTex = Resources.Load("Tex/Close") as Texture2D;


        StyleBlue = new GUIStyle();
        StyleBlue.normal.background = Blue;
        StyleBlueGreen = new GUIStyle();
        StyleBlueGreen.normal.background = BlueGreen;
        StyleGreen = new GUIStyle();
        StyleGreen.normal.background = Green;
        StyleGrey = new GUIStyle();
        StyleGrey.normal.background = Grey;
        StyleOrange = new GUIStyle();
        StyleOrange.normal.background = Orange;
        StyleRed = new GUIStyle();
        StyleRed.normal.background = Red;
        StyleRose = new GUIStyle();
        StyleRose.normal.background = Rose;
        StyleWhite = new GUIStyle();
        StyleWhite.normal.background = White;

        Style_threeNarrowWindows = new GUIStyle();
        Style_threeNarrowWindows.normal.background = threeNarrowWindows;
        Style_three_Windows = new GUIStyle();
        Style_three_Windows.normal.background = three_Windows;
        Style_sideAlleyEntrance = new GUIStyle();
        Style_sideAlleyEntrance.normal.background = sideAlleyEntrance;
        Style_shop6 = new GUIStyle();
        Style_shop6.normal.background = shop6;
        Style_shop5 = new GUIStyle();
        Style_shop5.normal.background = shop5;
        Style_shop4 = new GUIStyle();
        Style_shop4.normal.background = shop4;
        Style_shop3 = new GUIStyle();
        Style_shop3.normal.background = shop3;
        Style_shop2 = new GUIStyle();
        Style_shop2.normal.background = shop2;
        Style_shop1 = new GUIStyle();
        Style_shop1.normal.background = shop1;
        Style_LWindowSide = new GUIStyle();
        Style_LWindowSide.normal.background = LWindowSide;
        Style_garage = new GUIStyle();
        Style_garage.normal.background = garage;
        Style_Empty = new GUIStyle();
        Style_Empty.normal.background = Empty;
        Style_Empty_AC = new GUIStyle();
        Style_Empty_AC.normal.background = Empty_AC;
        Style_Door_Entrance = new GUIStyle();
        Style_Door_Entrance.normal.background = Door_Entrance;
        Style_Balcony = new GUIStyle();
        Style_Balcony.normal.background = Balcony;
        Style_Balcony03 = new GUIStyle();
        Style_Balcony03.normal.background = Balcony03;
        Style_Balcony02 = new GUIStyle();
        Style_Balcony02.normal.background = Balcony02;
        Style_Balcony01 = new GUIStyle();
        Style_Balcony01.normal.background = Balcony01;

        StyleStreet_CrossLine = new GUIStyle();
        StyleStreet_CrossLine.normal.background = Street_CrossLine;
        StyleStreet_Empty = new GUIStyle();
        StyleStreet_Empty.normal.background = Street_Empty;
        StyleStreet_Straight = new GUIStyle();
        StyleStreet_Straight.normal.background = Street_Straight;
        StyleStreet_Turn = new GUIStyle();
        StyleStreet_Turn.normal.background =Street_Turn ;

        Style_AddFloor = new GUIStyle();
        Style_AddFloor.normal.background = AddFloor;
        Style_RemoveFloor = new GUIStyle();
        Style_RemoveFloor.normal.background = RemoveFloor;
        Style_Building_delete = new GUIStyle();
        Style_Building_delete.normal.background = Building_delete;
        Style_MergeMesh = new GUIStyle();
        Style_MergeMesh.normal.background = MergeMesh;
        Style_RevertMeshMerge = new GUIStyle();
        Style_RevertMeshMerge.normal.background = RevertMeshMerge;
        Style_Street_Delete = new GUIStyle();
        Style_Street_Delete.normal.background = Street_delete;
        Style_Street_Rotate = new GUIStyle();
        Style_Street_Rotate.normal.background = Street_Rotate;

        Style_NewBuilding = new GUIStyle();
        Style_NewBuilding.normal.background = NewBuilding;
        Style_Close = new GUIStyle();
        Style_Close.normal.background = CloseTex;
    }
}
