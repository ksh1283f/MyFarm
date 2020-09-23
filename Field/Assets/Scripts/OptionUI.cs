using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionUI : MonoBehaviour
{
    [SerializeField] Text title;
    [SerializeField] Button btnExit;
    [SerializeField] Button btnTitle;

    void Start()
    {
        title.text = "Option";
        btnTitle.onClick.AddListener(onBtnTitle);
        btnExit.onClick.AddListener(onBtnExit);
    }

    void onBtnExit()
    {
        Application.Quit();
    }

    void onBtnTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
