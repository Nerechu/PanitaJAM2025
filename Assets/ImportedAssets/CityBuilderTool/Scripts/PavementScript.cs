using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PavementScript : MonoBehaviour
{
    public int Column, Row;
    public Transform PavementParent;
    public void SetRowAndColumns()
    {
        Column = (int)(transform.position.x / 6);
        Row = (int)(transform.position.z / 6);
    }
    public void RebuildPavement(bool Right, bool Left, bool forward, bool Backward)
    {
        //This function Remove the Existing Pavement
        //then based on The sides [Right,Left,forward,Backward] Choose The Pavement type and Rotation.
        ClearPavementParent();
        if (!Right && !Left && !forward && !Backward)
        {
            BuildPaveByNameAndRotate("FourFreeSides", 0);
        }
        if (Right && !Left && !forward && !Backward)
        {
            BuildPaveByNameAndRotate("ThreeFreeSides", 0);
        }
        if (!Right && !Left && forward && !Backward)
        {
            BuildPaveByNameAndRotate("ThreeFreeSides", 90);
        }
        if (!Right && Left && !forward && !Backward)
        {
            BuildPaveByNameAndRotate("ThreeFreeSides", 180);
        }
        if (!Right && !Left && !forward && Backward)
        {
            BuildPaveByNameAndRotate("ThreeFreeSides", 270);
        }
        if (Right && !Left && forward && !Backward)
        {
            BuildPaveByNameAndRotate("TwoFreeSides", 0);
        }
        if (!Right && Left && forward && !Backward)
        {
            BuildPaveByNameAndRotate("TwoFreeSides", 90);
        }
        if (!Right && Left && !forward && Backward)
        {
            BuildPaveByNameAndRotate("TwoFreeSides", 180);
        }
        if (Right && !Left && !forward && Backward)
        {
            BuildPaveByNameAndRotate("TwoFreeSides", 270);
        }
        if (Right && Left && !forward && !Backward)
        {
            BuildPaveByNameAndRotate("TwoFreeOppisiteSides", 0);
        }
        if (!Right && !Left && forward && Backward)
        {
            BuildPaveByNameAndRotate("TwoFreeOppisiteSides", 90);
        }
        if (Right && Left && forward && !Backward)
        {
            BuildPaveByNameAndRotate("OneFreeSide", 0);
        }
        if (!Right && Left && forward && Backward)
        {
            BuildPaveByNameAndRotate("OneFreeSide", 90);
        }
        if (Right && !Left && forward && Backward)
        {
            BuildPaveByNameAndRotate("OneFreeSide", 270);
        }
        if (Right && Left && !forward && Backward)
        {
            BuildPaveByNameAndRotate("OneFreeSide", 180);
        }
        if (Right && Left && forward && Backward)
        {
            BuildPaveByNameAndRotate("NoSides", 0);
        }
    }

    private void BuildPaveByNameAndRotate(string PaveName, float rotation)
    {
        //here Create a new gameobject and Set its rotation.
        GameObject g = Instantiate(Resources.Load(PaveName)) as GameObject;
        g.transform.parent = PavementParent;
        g.transform.localPosition = Vector3.zero;
        g.transform.localEulerAngles = Vector3.forward * rotation;
        //
    }

    private void ClearPavementParent()
    {
        //Delete the pavement to create New Pavement.
        if (PavementParent.childCount > 0)
        {
            for (int i = 0; i < PavementParent.childCount; i++)
            {
                DestroyImmediate(PavementParent.GetChild(i).gameObject);
            }
        }
    }
}