using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayTime : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI nick;
    [SerializeField] Image[] stars; // Assuming you have an array of Image components for the stars
    private float playerTime;
    public void UpdateTime(int remainingTime)
    {
        playerTime = 300 - remainingTime; //Assuming score is in seconds, 300 seconds = 5 minutes
        int minutes = Mathf.FloorToInt(playerTime / 60);
        int seconds = Mathf.FloorToInt(playerTime % 60);
        time.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        //tengo que aprender a hacer que guarden el nombre de usuario.
        //nick.text = name;
    }

    public void UpdateStars(int stars)
    {
        if (stars == 3)
        {
            // Update UI to show 3 star
            this.stars[2].enabled = true; // Enable the third star
            this.stars[1].enabled = true; // Enable the second star
            this.stars[0].enabled = true; // Enable the first star
        }
        else if (stars == 2)
        {
            // Update UI to show 2 stars
            this.stars[1].enabled = true; // Enable the second star
            this.stars[0].enabled = true; // Enable the first star
        }
        else if (stars == 1)
        {
            // Update UI to show 1 stars
            this.stars[0].enabled = true; // Enable the first star
        }
    }
}
