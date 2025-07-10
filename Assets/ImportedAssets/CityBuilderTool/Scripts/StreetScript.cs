using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetScript : MonoBehaviour
{
    //This Script will hold the Type and Position in row and column.

    public StreetBlockType currentType;
    public int col, row;

    public enum StreetBlockType
    {
        Empty,straight,turn,crossline
    }

    public void SetRowAndColumns(int column, int roww)
    {
        col = column;
        row=roww;
    }
}