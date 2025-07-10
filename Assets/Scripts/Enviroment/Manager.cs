using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public int time;
    public static Manager instance;
    [SerializeField]DisplaySlider displaySlider; // Reference to the DisplaySlider script
    int totalPlants = 0;
    int currentPlants = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // Sets the static instance
            DontDestroyOnLoad(gameObject); // Keeps this object across scenes
        }
        else
        {
            Destroy(gameObject); // Destroys duplicate instances
        }
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
        time = Mathf.FloorToInt(TimerSystem.instance.remainingTime);
        SceneManager.LoadScene("Win");
    }
}
