using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    public float remainingTime = 5f;
    public static TimerSystem instance;

    private void Awake()
    {
        instance = this; // Sets the static instance
    }

    void Update()
    {
        if(remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime <= 0)
        {
            remainingTime = 0;
            Manager.instance.Lose(); // Call the Lose method from Manager when time runs out
        }
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
