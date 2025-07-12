using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene : MonoBehaviour
{
    private bool animationPlayed = false;

    private PlayableDirector pd;


    private static Cutscene instance;

    private void OnEnable()
    {
        pd.stopped += OnPlayableDirectorStopped;
    }
    private void Awake()
    {
        pd = GetComponent<PlayableDirector>();
        if (instance == null)
        {
            instance = this;
        }

        else Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        

        if (animationPlayed)
        {
            this.gameObject.SetActive(false);  

        }
        animationPlayed = true;
    }

    void OnPlayableDirectorStopped(PlayableDirector p)
    {
        p.gameObject.SetActive(false);

    }
}
