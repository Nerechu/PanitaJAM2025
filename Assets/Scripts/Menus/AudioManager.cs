using UnityEngine;
using System;

public enum SoundType
{
    MUSIC,
    WALK,
    RUN,
    JUMP,
    DASH,
    WALLRUN,
    CLIMB,
    FIRESEED,
    FIREHOOK,
    SEEDPLANTED,
    SEEDMISSED,
    HOOKLANDED,
    HOOKMISSED,
    HOOKRELEASE,
    BUTTONHOVER,
    BUTTONCLICK,
    VICTORY,
    DEFEAT,
    DEATH,
    PLANTGROWTH
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource audioSource;
    public AudioSource musicSource;

    [Header("---------- Sounds ----------")]

    [SerializeField] private SoundsList[] soundList;

    //estaba probando pasandole una lista de un script serializable, y pasar nombres en vez de toda la instancia con el audioclip.
    //public Sound[] musicSounds, sfxSounds;
    
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds; 
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, volume);
        
        //audioSource.PlayOneShot(sound);
    }

    public void PlayDelayedSound(SoundType sound, float volume = 1, float delay = 0)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.clip = randomClip;
        instance.audioSource.PlayDelayed(delay);

        //audioSource.PlayOneShot(sound);
    }

    public void PlayMusic(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        //AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(clips[0], volume);

        //audioSource.PlayOneShot(sound);
    }

    public void SFXVolume(float volume)
    {
        audioSource.volume = volume;
    }
    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }


#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif

}

[Serializable]
public struct SoundsList
{
    public AudioClip[] Sounds {  get => sounds;}
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}
