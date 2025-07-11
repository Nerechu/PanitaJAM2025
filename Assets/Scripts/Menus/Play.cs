using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Play : MonoBehaviour
{
    [SerializeField] private Image background;
    public void PlayGame()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ChangeBackground()
    {
        Color32 dark = new Color32(80, 80, 80, 255);
        background.color = dark;
    }

    public void ResetBackground()
    {
        Color32 light = new Color32(255, 255, 255, 255);
        background.color = light;
    }
}
