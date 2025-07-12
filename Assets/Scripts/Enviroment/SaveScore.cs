using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveScore : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI myName;
    bool firstTime = true;
    public void SaveTime()
    {
        if (firstTime == true)
        {
            firstTime = false;
            Debug.Log($"Name: {myName.text}, Time: {Manager.instance.time}");
            HighScores.UploadScore(myName.text, Manager.instance.time);
        }
    }
}
