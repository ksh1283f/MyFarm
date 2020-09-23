using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenImage : MonoBehaviour
{
    public TYPE ItemType;
    public Text TextItemInfo;
    public Button btnUse;
    GameStatus gameStatus = null;
    private void Start()
    {
        gameStatus = GameObject.Find("GameRoot").GetComponent<GameStatus>();
        btnUse = GetComponent<Button>();
        if (btnUse != null)
        {
            btnUse.onClick.AddListener(()=> { gameStatus.OnClickedSeedBtn(ItemType); });
        }
    }
}
