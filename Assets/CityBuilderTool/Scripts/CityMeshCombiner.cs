using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityMeshCombiner : MonoBehaviour
{
    public void CombineCity()
    {
        if (GetComponent<CityScript>() != null)
        {
            CityScript CS = GetComponent<CityScript>();
            if (CS.buildings.Count > 0)
            {
                for (int i = 0; i < CS.buildings.Count; i++)
                {
                    CS.buildings[i].GetComponent<MeshCombiner>().CombineAllMeshes();
                }
            }
            GameObject StreetParent = GameObject.FindGameObjectWithTag("StreetParent");
            StreetParent.AddComponent<MeshCombiner>();
            MeshCombiner street_mesh_combiner = StreetParent.GetComponent<MeshCombiner>();
            street_mesh_combiner.CombineAllMeshes();
        }
    }

    public void RevertCity()
    {
        if (GetComponent<CityScript>() != null)
        {
            CityScript CS = GetComponent<CityScript>();
            if (CS.buildings.Count > 0)
            {
                for (int i = 0; i < CS.buildings.Count; i++)
                {
                    CS.buildings[i].GetComponent<MeshCombiner>().RevertCombination();
                }
            }
            GameObject StreetParent = GameObject.FindGameObjectWithTag("StreetParent");
            MeshCombiner street_mesh_combiner = StreetParent.GetComponent<MeshCombiner>();
            street_mesh_combiner.RevertCombination();
        }
    }
}
