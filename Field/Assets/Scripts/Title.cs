using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] Button btnStart;
    [SerializeField] Button btnExit;
    [SerializeField] Text maxScoreText;
    AudioSource effect;
    MaxScore maxScore;
    private void Start()
    {
        maxScore = GameObject.Find("MaxScore").GetComponent<MaxScore>();
        effect = GetComponent<AudioSource>();
        btnStart.onClick.AddListener(OnStart);
        btnExit.onClick.AddListener(OnExit);
        SetMaxScore(maxScore.GetMaxScore());
    }

    void OnStart()
    {
        effect.Play();
        SceneManager.LoadScene("GameScene");
    }

    void OnExit()
    {
        effect.Play();
        Application.Quit();
    }

    void SetMaxScore(int score)
    {
        if (score == -1)
            maxScoreText.text = string.Concat("No max record");
        else
            maxScoreText.text = string.Concat("max money: ", score);
    }
}
