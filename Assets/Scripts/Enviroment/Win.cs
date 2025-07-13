using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.BoolParameter;

public class Win : MonoBehaviour
{
    [SerializeField] GameObject winCanvas; // Reference to the Game Win panel
    [SerializeField] GameObject levelCanvas; // Reference to the Game level panel
    [SerializeField] GameObject player; // Reference to the player GameObject

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Manager.instance.Win(); // Call the Win method from the Manager instance    
        }
    }
}

