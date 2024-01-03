using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Text txtScore;

    private int totScore = 0;

    void Start()
    {
        totScore = PlayerPrefs.GetInt("TOT_SCORE", 0);

        DispScore(0);
    }

    public void DispScore(int score)
    {
        totScore += score;
        txtScore.text = "Score <color=#ff0000>" + totScore.ToString() + "</color>";

        PlayerPrefs.SetInt("TOT_SCORE", totScore);
    }
}
