using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    float elapsedTime = 0f;
    [SerializeField] float remainingTime = 300f;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if(remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime <= 0)
        {
            remainingTime = 0;
        }
        UpdateTimerText();
        CheckLose();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void CheckLose()
    {
        if (remainingTime <= 0) // 5 min in seconds in inspector
        {
            //Debug.Log("Time's up! You lose!");
            SceneManager.LoadScene("Lose");
        }
    }
}
