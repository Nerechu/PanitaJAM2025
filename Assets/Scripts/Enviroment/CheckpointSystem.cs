using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Checkpoint> CheckpointList;
    public static CheckpointSystem instance;
    public GameObject Player;

    private void Awake()
    {
        instance = this;
    }
    public void Respawn()
    {
        Player.transform.position = CheckpointList[CheckpointList.Count - 1].transform.position;
    }

    public void AddCheckpoint(Checkpoint checkpoint)
    {
        if (!CheckpointList.Contains(checkpoint))
            CheckpointList.Add(checkpoint);
    }


}
