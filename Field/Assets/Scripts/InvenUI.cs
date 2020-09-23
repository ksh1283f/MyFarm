using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Text;

public class InvenUI : MonoBehaviour
{
    public Text turnText;
    public Text GameInfo;
    public List<InvenImage> invenImageList = new List<InvenImage>();
    public List<InvenImage> toolImageList = new List<InvenImage>();
    public GameStatus gameStatus = null;
    public OptionUI optionUI;

    private void Awake()
    {
        gameStatus = GameObject.Find("GameRoot").GetComponent<GameStatus>();
        gameStatus.OnUpdatedItemList += OnUpdateInven;
        gameStatus.OnChangedTurn += SetText;
        gameStatus.OnUpdateGameInfo += OnUpdateUI;
    }

    private void Start()
    {
        SetText(gameStatus.RemainTurn);
        InitToolImages();
        optionUI.gameObject.SetActive(false);
    }

    void InitToolImages()
    {
        toolImageList[0].TextItemInfo.text = "Axe";
        toolImageList[1].TextItemInfo.text = "Sprinkler";
    }

    void OnUpdateInven(Dictionary<TYPE, int> list)
    {
        for (int i = 0; i < invenImageList.Count; i++)
        {
            foreach (var item in list)
            {
                if (invenImageList[i].ItemType == item.Key)
                {
                    updateInfo(invenImageList[i], item.Key, item.Value);
                    break;
                }
            }
        }
    }

    void OnUpdateUI(int hp, int gold, int lumber, int penalty)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("기력: ");
        sb.Append(hp);
        sb.Append("     돈: ");
        sb.Append(gold);
        sb.Append("     나무: ");
        sb.Append(lumber);
        sb.Append("     패널티: ");
        sb.Append(penalty);
        GameInfo.text = sb.ToString();
    }

    void updateInfo(InvenImage image, TYPE type, int count)
    {
        if (image == null)
            return;

        string infoText = string.Empty;
        switch (type)
        {
            case TYPE.OSeed:
            case TYPE.SSeed:
                infoText = string.Concat(type.ToString(), "\n", count, "\nINFINITE");
                image.TextItemInfo.text = infoText;
                break;

            case TYPE.Lumber:
            case TYPE.Macintosh:
            case TYPE.Corn:
            case TYPE.Tomato:
            case TYPE.Grape:
            case TYPE.MSeed:
            case TYPE.CSeed:
            case TYPE.Orange:
            case TYPE.Strawberry:
            case TYPE.TSeed:
            case TYPE.GSeed:
                infoText = string.Concat(type.ToString(), "\n", count);
                image.TextItemInfo.text = infoText;
                break;
        }

        if (string.IsNullOrEmpty(infoText))
            Debug.LogError("invalid type: " + type.ToString());
    }

    void SetText(int count)
    {
        if (turnText == null)
            return;

        turnText.text = string.Concat("남은 턴: ", count.ToString());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionUI.gameObject.SetActive(!optionUI.gameObject.activeSelf);
        }
    }
}
