using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public float time;
    public static Manager instance;
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

    public void Win()
    {
        time = TimerSystem.instance.remainingTime;
        SceneManager.LoadScene("Win");
    }
}
