using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mail : MonoBehaviour
{
    GameStatus gameStatus = null;

    private void Start()
    {
        gameStatus = GameObject.Find("GameRoot").GetComponent<GameStatus>();
    }

    /// <summary>
    ///  인벤에 있는 팔수있는 아이템들 다 팔기
    /// </summary>
    /// <param name="inven"></param>
    public void Sell(List<Tuple<ItemData, int>> inven)
    {

    }

}
