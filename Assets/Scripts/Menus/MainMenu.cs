using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {

        SceneManager.LoadScene(1);
    }

    public void LVL2()
    {

        SceneManager.LoadScene(2);
    }

    public void QuitGame() 
    {
        Debug.Log("Quit!");
        Application.Quit();  
    }
}
