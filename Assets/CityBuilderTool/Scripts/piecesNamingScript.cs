using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class piecesNamingScript : MonoBehaviour
{
    //This Script make sure (no misspelling) happen.
    public string getPieceName(BuildingPiece piece, ColorName Piececolor)
    {
        switch (piece)
        {
            case BuildingPiece.CM_Floor_Ring_:
                return "Floor_Ring_" + getColorName(Piececolor);
            case BuildingPiece.CM_topCeiling:
                return "topCeiling" + getColorName(Piececolor);
            case BuildingPiece.CM_TopFloorDoor_:
                return "TopFloorDoor_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_Balcony01_:
                return "Balcony01_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_Balcony02_:
                return "Balcony02_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_Balcony03_:
                return "Balcony03_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_Balcony_:
                return "Balcony_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_Door_Entrance_:
                return "Door_Entrance_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_Empty_:
                return "Empty_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_Empty_AC_:
                return "Empty_AC_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_garage_:
                return "garage_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_LWindowSide_:
                return "LWindowSide_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_shop1_:
                return "shop1_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_shop2_:
                return "shop2_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_shop3_:
                return "shop3_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_shop4_:
                return "shop4_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_shop5_:
                return "shop5_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_shop6_:
                return "shop6_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_sideAlleyEntrance_:
                return "Alley_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_threeNarrowWindows_:
                return "threeWindows_" + getColorName(Piececolor);
            case BuildingPiece.CM_Wallside_three_Windows_:
                return "three_Windows_" + getColorName(Piececolor);
            case BuildingPiece.CM_Top_Ring_:
                return "Top_Ring_" + getColorName(Piececolor);
            default:
                return "Empty_Orange";
        }
    }
    public static string getColorName(ColorName Piececolor)
    {
        switch (Piececolor)
        {
            case ColorName.Cyan:
                return "Cyan";
            case ColorName.Bluo:
                return "Blue";
            case ColorName.Green:
                return "Green";
            case ColorName.Grey:
                return "Grey";
            case ColorName.Orange:
                return "Orange";
            case ColorName.Red:
                return "Red";
            case ColorName.Rose:
                return "Rose";
            default:
                return "Orange";
        }
    }

  
}
  public enum BuildingPiece
    {
        CM_Floor_Ring_, CM_topCeiling, CM_TopFloorDoor_, CM_Wallside_Balcony01_, CM_Wallside_Balcony02_, CM_Wallside_Balcony03_, CM_Wallside_Balcony_, CM_Wallside_Door_Entrance_, CM_Wallside_Empty_AC_, CM_Wallside_Empty_, CM_Wallside_garage_, CM_Wallside_LWindowSide_, CM_Wallside_shop1_, CM_Wallside_shop2_, CM_Wallside_shop3_, CM_Wallside_shop4_, CM_Wallside_shop5_, CM_Wallside_shop6_, CM_Wallside_sideAlleyEntrance_, CM_Wallside_three_Windows_, CM_Wallside_threeNarrowWindows_, CM_Top_Ring_,
    }
    public enum ColorName
    {
        Cyan, Bluo, Green, Grey, Orange, Red, Rose
    }