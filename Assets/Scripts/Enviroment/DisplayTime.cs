using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayTime : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI nick;
    private float playerTime;
    void Start()
    {
        playerTime = 300 - Manager.instance.time; //Assuming score is in seconds, 300 seconds = 5 minutes
        int minutes = Mathf.FloorToInt(playerTime / 60);
        int seconds = Mathf.FloorToInt(playerTime % 60);
        time.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        //tengo que aprender a hacer que guarden el nombre de usuario.
        //nick.text = name;
    }
}
