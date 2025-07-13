using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public GameObject seedPrefav;
    bool isPlanted = false;
    void Awake()
    {
        Manager.instance.addPlant();
    }

    
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("OnTriggerEnter called with: " + other.gameObject.name);
        //seedPrefav check
        if (other.gameObject.CompareTag("Seed") && isPlanted == false)
        {
            Debug.Log("Seed Prefab detected: " + other.gameObject.name);
            isPlanted = true;
            Manager.instance.subPlant();
        }
    }
}
