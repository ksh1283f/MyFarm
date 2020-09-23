using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [SerializeField] Button btnTitle;
    [SerializeField] Button btnExit;
    [SerializeField] Text resultText;
    MaxScore maxScore;
    AudioSource effect;

    void Start()
    {
        maxScore = GameObject.Find("MaxScore").GetComponent<MaxScore>();
        effect = GetComponent<AudioSource>();
        btnTitle.onClick.AddListener(OnTitle);
        btnExit.onClick.AddListener(OnExit);
        SetResultText();
    }

    void OnTitle()
    {
        effect.Play();
        SceneManager.LoadScene("TitleScene");
    }

    void OnExit()
    {
        effect.Play();
        Application.Quit();
    }

    void SetResultText()
    {
        string message = string.Concat("Your record: ", maxScore.presentRecord);
        if (maxScore.GetMaxScore() != -1)
            message = string.Concat(message, "\n max record: ", maxScore.GetMaxScore());

        if (maxScore.presentRecord > maxScore.GetMaxScore())
        {
            message= string.Concat(message, "\n new max record! \n Congratulation!!");
            maxScore.BreakMaxRecord();
        }

        resultText.text = message;
    }
}
