using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHighscores : MonoBehaviour 
{
    public TMPro.TextMeshProUGUI[] rNames;
    public TMPro.TextMeshProUGUI[] rScores;
    HighScores myScores;

    void Start() //Fetches the Data at the beginning
    {
        for (int i = 0; i < rNames.Length;i ++)
        {
            rNames[i].text = i + 1 + ". Fetching...";
        }
        myScores = GetComponent<HighScores>();
        StartCoroutine("RefreshHighscores");
    }
    public void SetScoresToMenu(PlayerScore[] highscoreList) //Assigns proper name and score for each text value
    {
        for (int i = 0; i < rNames.Length;i ++)
        {
            rNames[i].text = i + 1 + ". ";
            if (highscoreList.Length > i)
            {
                //AQUI CAMBIAR PARA CALCULAR EL TIEMPO
                int remainingTime = 300 - highscoreList[i].score; //Assuming score is in seconds, 300 seconds = 5 minutes
                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);
                rScores[i].text = string.Format("{0:00}:{1:00}", minutes, seconds);
                string name = (i + 1) + ". " + highscoreList[i].username;
                rNames[i].text = name;
            }
        }
    }
    IEnumerator RefreshHighscores() //Refreshes the scores every 30 seconds
    {
        while(true)
        {
            myScores.DownloadScores();
            yield return new WaitForSeconds(30);
        }
    }
}
