using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public enum stateGame { STOP = 0, CONTINUE = 1 };
    public stateGame currentState = stateGame.CONTINUE;

    public AudioSource music;
    
    public Slider volumeMusicSlider;
    public Slider volumeFXSlider;
    public AudioMixer audioMixer;
    public AudioMixer audioMixer2;

    void Awake()
    {
        //audioMixer.SetFloat("MusicVolumen", SaveSettings.Instance.musicVolum);
        //audioMixer2.SetFloat("FXVolumen", SaveSettings.Instance.fxVolum);

       // volumeMusicSlider.value = SaveSettings.Instance.musicVolum;
       // volumeFXSlider.value = SaveSettings.Instance.fxVolum;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (currentState == stateGame.CONTINUE)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

    }
    public void PauseGame()
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        currentState = stateGame.STOP;
        Time.timeScale = 0; 
        if (music != null)
        {
            music.Pause(); 
        }
    }

    public void ResumeGame()
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);

        currentState = stateGame.CONTINUE;
        Time.timeScale = 1; 
        if (music != null)
        {
            music.Play(); 
        }
    }

    public void Home()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    public void Restart()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetMusicAudio(float musicValue)
    {
        SaveSettings.Instance.musicVolum = musicValue;
        audioMixer.SetFloat("MusicVolumen", musicValue);
    }

    public void SetFXAudio(float FXValue)
    {
        SaveSettings.Instance.fxVolum = FXValue;
        audioMixer2.SetFloat("FXVolumen", FXValue);
    }
}
