using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip mainMenuMusic;
    void Start()
    {
        Debug.Log("PlayingMAinMenuMusic");
        audioSource.loop = true;
        audioSource.PlayOneShot(mainMenuMusic);
    }

}
