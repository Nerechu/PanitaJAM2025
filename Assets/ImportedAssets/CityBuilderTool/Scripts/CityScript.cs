using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CityScript : MonoBehaviour
{
    [HideInInspector]
    public List<PavementScript> buildings = new List<PavementScript>();
    [HideInInspector]
    public List<StreetScript> streetBlocks = new List<StreetScript>();
    bool Right, Left, Backward, forward;
    bool SurRight, SurLeft, SurBackward, Surforward;
    int currentCell;
    public Transform StreetParent;

    public void ReCheckBuildings(int column, int row)
    {
        //Clears the buildings array and  Re-Build it.
        buildings.Clear();
        //Clears the Streetblocks array and  Re-Build it.
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<PavementScript>() != null)
            {
                buildings.Add(transform.GetChild(i).GetComponent<PavementScript>());
            }
        }

        updatePaveMentAround(column, row);
    }

    public void ReCheckAndBuild_Streets(int column, int row)
    {
        //Clears the Streetblocks array and  Re-Build it.
        StreetParent = GameObject.FindGameObjectWithTag("StreetParent").transform;
        streetBlocks.Clear();
        for (int i = 0; i < StreetParent.childCount; i++)
        {
            if (StreetParent.GetChild(i).GetComponent<StreetScript>() != null)
            {
                streetBlocks.Add(StreetParent.GetChild(i).GetComponent<StreetScript>());
            }
        }
    }

    private void updatePaveMentAround(int column, int row)
    {
        //This function update the pavement of the surrounding cells. 
        Right = false;
        Left = false;
        Backward = false;
        forward = false;
        currentCell = 0;
        ReCheckRightLeftForwardBackwardsAndCurrentCell(column, row);
        buildings[currentCell].RebuildPavement(Right, Left, forward, Backward);
        updateSurroundingCellsAround(column, row + 1);
        updateSurroundingCellsAround(column, row - 1);
        updateSurroundingCellsAround(column + 1, row);
        updateSurroundingCellsAround(column - 1, row);
    }

    private void updateSurroundingCellsAround(int column, int row)
    {
        //This function update the pavement the surrounding cells at (column,row). 
        int index = GetindexByColumnAndRowBuildings(column, row);
        if (index == -1) { return; }
        SurRight = false;
        SurLeft = false;
        SurBackward = false;
        Surforward = false;
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].Column == buildings[index].Column - 1 && buildings[i].Row == buildings[index].Row)
            {
                SurBackward = true;
            }
            if (buildings[i].Column == buildings[index].Column + 1 && buildings[i].Row == buildings[index].Row)
            {
                Surforward = true;
            }
            if (buildings[i].Row == buildings[index].Row - 1 && buildings[i].Column == buildings[index].Column)
            {
                SurLeft = true;
            }
            if (buildings[i].Row == buildings[index].Row + 1 && buildings[i].Column == buildings[index].Column)
            {
                SurRight = true;
            }
        }
        buildings[index].RebuildPavement(SurRight, SurLeft, Surforward, SurBackward);
    }

    public void RemoveAndUpdateBuildings(int column, int row)
    {
        //Remove old Building Block Build a new one 

        int index = GetindexByColumnAndRowBuildings(column, row);
        if (index == -1) { return; }

        DestroyImmediate(buildings[index].gameObject);
        buildings.RemoveAt(index);

        updateSurroundingCellsAround(column + 1, row);
        updateSurroundingCellsAround(column - 1, row);
        updateSurroundingCellsAround(column, row + 1);
        updateSurroundingCellsAround(column, row - 1);
    }

    public void RemoveAndUpdateStreet(int column, int row)
    {
        //Remove old street Block Build a new one 
        int index = GetindexByColumnAndRowStreets(column, row);
        if (index == -1) { return; }
        DestroyImmediate(streetBlocks[index].gameObject);
        streetBlocks.RemoveAt(index);
        ReCheckAndBuild_Streets(column, row);
    }

    public bool CheckEmptyCell(int column, int row)
    {
        // this Check If the cell(column,row) is empty (no streets ,no Buildings)
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].Column == column)
            {
                if (buildings[i].Row == row)
                {
                    return false;
                }
            }
        }
        for (int i = 0; i < streetBlocks.Count; i++)
        {
            if (streetBlocks[i].col == column)
            {
                if (streetBlocks[i].row == row)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public int GetindexByColumnAndRowBuildings(int column, int row)
    {
        // this function return the index of the building block based on Column and row.
        int index = -1;
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].Column == column && buildings[i].Row == row)
            {
                index = i;
            }
        }
        return index;
    }

    public int GetindexByColumnAndRowStreets(int column, int row)
    {
        // this function return the index of the street block based on Column and row.

        int index = -1;
        for (int i = 0; i < streetBlocks.Count; i++)
        {
            if (streetBlocks[i].col == column && streetBlocks[i].row == row)
            {
                index = i;
            }
        }
        return index;
    }

    void ReCheckRightLeftForwardBackwardsAndCurrentCell(int column, int row)
    {
        //this function Check the surrounding to set (Right,Lef,Backward,forward)
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].Column == column - 1 && buildings[i].Row == row)
            {
                Backward = true;
            }
            if (buildings[i].Column == column + 1 && buildings[i].Row == row)
            {
                forward = true;
            }
            if (buildings[i].Row == row - 1 && buildings[i].Column == column)
            {
                Left = true;
            }
            if (buildings[i].Row == row + 1 && buildings[i].Column == column)
            {
                Right = true;
            }
            if (buildings[i].Row == row && buildings[i].Column == column)
            {
                currentCell = i;
            }
        }
    }
}
