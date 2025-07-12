using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public int time;
    int totalPlants = 0;
    int currentPlants = 0;
    public static Manager instance;
    [SerializeField] GameObject winCanvas; // Reference to the Game Win panel
    [SerializeField] GameObject loseCanvas; // Reference to the Game Lose panel
    [SerializeField] GameObject levelCanvas; // Reference to the Game level panel
    [SerializeField] DisplaySlider displaySlider; // Reference to the DisplaySlider script
    [SerializeField] DisplayTime displayTime; // Reference to the DisplaySlider script
    [SerializeField] int firstStar = 200; // Time in seconds to achieve the first star
    [SerializeField] int secondStar = 150; // Time in seconds to achieve the second star
    [SerializeField] int thirdStar = 100; // Time in seconds to achieve the third star


    private void Awake()
    {
        instance = this; // Sets the static instance
    }
    private void Start()
    {
        displaySlider.UpdateUI(currentPlants, totalPlants); // Update the UI with the current and total plants
    }

    public void addPlant()
    {
        totalPlants++;
    }

    public void subPlant()
    {
        currentPlants++;
        displaySlider.UpdateUI(currentPlants, totalPlants); // Update the UI with the current and total plants

        if (currentPlants >= totalPlants && totalPlants > 0)
        {
            Win();
        }
    }

    public void Win()
    {
        winCanvas.SetActive(true); // Show the win panel
        levelCanvas.SetActive(false); // Hide the level panel
        Time.timeScale = 0f;
        time = Mathf.FloorToInt(TimerSystem.instance.remainingTime);
        displayTime.UpdateTime(time);
        time = 300 - time;
        if (time < thirdStar)
        {
            displayTime.UpdateStars(3); // 3 stars awarded
        }
        else if (time < secondStar)
        {
            displayTime.UpdateStars(2); // 2 stars awarded
        }
        else if (time < firstStar)
        {
            displayTime.UpdateStars(1); // 1 star awarded
        }
        else
        {
            displayTime.UpdateStars(0); // No stars awarded
        }
    }

    public void Lose()
    {
        loseCanvas.SetActive(true); // Show the lose panel
        levelCanvas.SetActive(false); // Hide the level panel
        Time.timeScale = 0f;
    }
}
