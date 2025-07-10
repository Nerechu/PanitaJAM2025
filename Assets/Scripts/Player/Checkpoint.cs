using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform respawnPoint;

    private void Start()
    {
        respawnPoint = GetComponentInChildren<Transform>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
        Debug.Log(other.tag);
        if (other.CompareTag("Player"))
        {
            CheckpointSystem.instance.AddCheckpoint(this);
        }
    }
}
