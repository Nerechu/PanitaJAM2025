using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{
    public void RetryScene()
    {
        SceneManager.LoadScene("Lvl1");
    }

    public void MainMenuScene()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
