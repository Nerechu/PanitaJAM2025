using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

public class Cutscene : MonoBehaviour
{
    private bool animationPlayed = false;
    private PlayableDirector pd;

    [Header("GameObjects que contienen scripts a desactivar")]
    [Tooltip("GameObjects del jugador que contienen scripts de movimiento, input o disparo")]
    [SerializeField] private GameObject[] objectsWithComponentsToDisable;

    [Header("Timer a pausar")]
    [SerializeField] private GameObject hudTimerObject;

    private List<MonoBehaviour> componentsToDisable = new List<MonoBehaviour>();
    private static Cutscene instance;

    private void Awake()
    {
        pd = GetComponent<PlayableDirector>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnEnable()
    {
        pd.stopped += OnPlayableDirectorStopped;
    }

    void Start()
    {
        if (animationPlayed)
        {
            gameObject.SetActive(false);
            return;
        }

        animationPlayed = true;

        // 🔍 Buscar y desactivar componentes
        foreach (var go in objectsWithComponentsToDisable)
        {
            if (go != null)
            {
                var scripts = go.GetComponents<MonoBehaviour>();
                foreach (var script in scripts)
                {
                    if (script != null && script.enabled)
                    {
                        componentsToDisable.Add(script);
                        script.enabled = false;
                    }
                }
            }
        }

        // ⏸️ Pausar el timer
        if (hudTimerObject != null)
            hudTimerObject.SetActive(false);

        pd.Play();
    }

    void OnPlayableDirectorStopped(PlayableDirector p)
    {
        // 🔓 Reactivar componentes
        foreach (var comp in componentsToDisable)
        {
            if (comp != null)
                comp.enabled = true;
        }

        // ▶️ Reanudar timer
        if (hudTimerObject != null)
            hudTimerObject.SetActive(true);

        gameObject.SetActive(false);
    }
}
