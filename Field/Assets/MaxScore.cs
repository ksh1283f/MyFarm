using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxScore : MonoBehaviour
{
    private const string MAX_SCORE = "MAX_SCORE";
    public int presentRecord { get; private set; }

    public int GetMaxScore()
    {
        return PlayerPrefs.GetInt(MAX_SCORE, -1);
    }

    public void SetScore(int score)
    {
        //if(GetMaxScore()<presentRecord)
        //    PlayerPrefs.SetInt(MAX_SCORE, score);
        presentRecord = score;
    }

    public void ResetPresentRecord()
    {
        presentRecord = 0;
    }

    public void BreakMaxRecord()
    {
        if (GetMaxScore() < presentRecord)
            PlayerPrefs.SetInt(MAX_SCORE, presentRecord);
    }
}
