using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject

    public void RetryScene()
    {
        Time.timeScale = 1;
        player.GetComponent<ParkourFPS.PlayerControllerScript>().enabled = true; // Disable player controls
        player.GetComponent<PlantGun>().enabled = true; // Disable player controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("Lvl1");
    }

    public void MainMenuScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }
}
