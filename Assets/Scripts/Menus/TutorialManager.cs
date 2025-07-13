using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] images;
    public GameObject panel;

    public float maxAlpha;
    public float tutorialTimerSeconds;

    private bool tutorialTimeFinished = false;
    private void Start()
    {
        StartCoroutine(increaseAlpha());
    }

    private void Update()
    {
        
        if (tutorialTimerSeconds < 0 && !tutorialTimeFinished)
        {
            tutorialTimeFinished = true;
            StopAllCoroutines();
            StartCoroutine(decreaseAlpha());
        }
        else if (tutorialTimerSeconds > 0)
        {
            tutorialTimerSeconds -= Time.deltaTime;
        }
    }


    IEnumerator increaseAlpha ()
    {
        float alpha = 0;
        while (alpha < maxAlpha)
        {

            alpha += 0.075f;
            foreach (GameObject i in images) {
                i.GetComponent<Image>().color = new Color (1,1,1,alpha);
            }
            panel.GetComponent<Image>().color = new Color(0, 0, 0, alpha/2);

            yield return new WaitForSeconds(.1f);
        
        }

        yield return null;
    }

    IEnumerator decreaseAlpha ()
    {
        float alpha = maxAlpha;
        while (alpha > 0)
        {
            Debug.Log(alpha);
            alpha -= 0.075f;
            foreach (GameObject i in images)
            {
                i.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
            }
            panel.GetComponent<Image>().color = new Color(0, 0, 0, alpha / 2);
            yield return new WaitForSeconds(.1f);
        }

        yield return null;
    }


}
