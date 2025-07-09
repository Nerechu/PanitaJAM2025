using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FloorsEditor : MonoBehaviour
{
    public int FloorsCount = 2;
    public Transform floorParent;
    public Transform TopCeiling;

    public void UpdateFloors()
    {
        //This Function will remove or add new Floors
        //then reposition topCeilling.

        if (floorParent.childCount != FloorsCount)
        {
            if (floorParent.childCount > FloorsCount)
            {
                while (floorParent.childCount != FloorsCount)
                {
                    //Remove Extra floors.
                    //(floorParent.childCount - 1) last floor.
                    DestroyImmediate(floorParent.GetChild(floorParent.childCount - 1).gameObject);
                    TopCeiling.transform.localPosition = Vector3.up * 2.4f * (floorParent.childCount);
                }
            }
            else
            {
                while (floorParent.childCount != FloorsCount)
                {
                    //add floors.
                    GameObject g = Instantiate(Resources.Load("Floor")) as GameObject;
                    g.transform.parent = floorParent;
                    g.transform.localPosition = Vector3.up * 2.4f * (floorParent.childCount - 1);
                    TopCeiling.transform.localPosition = Vector3.up * 2.4f * (floorParent.childCount);
                }
            }

        }
    }
}