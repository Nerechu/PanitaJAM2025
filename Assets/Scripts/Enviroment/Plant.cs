using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public GameObject seedPrefav;
    bool isPlanted = false;
    private void Awake()
    {
        Manager.instance.addPlant();
    }

    
    private void OnTriggerEnter(Collider other)
    {
        //seedPrefav check
        if (other.gameObject == seedPrefav && !isPlanted)
        {
            isPlanted = true;
            Manager.instance.subPlant();
        }
    }
}
