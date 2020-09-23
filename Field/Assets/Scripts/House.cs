using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    public GameStatus gameStatus = null;
    public int penalty = 0;

    void Start()
    {
        gameStatus = GameObject.Find("GameRoot").GetComponent<GameStatus>();
        gameStatus.OnChangedTurn += Sleep;
        gameStatus.Penalty = penalty;
    }

    void Sleep(int count)
    {
        // 나무 저장량에 맞는 체력회복
        if(gameStatus.RemainLumberCount<=0)
            penalty++;

        if(penalty >= 8)
            penalty = 8;
        gameStatus.Hp = 100 - (penalty * 10);

        gameStatus.Penalty = penalty;
        // 나무 저장량 감소
        if (gameStatus.RemainLumberCount > 0)
            gameStatus.RemainLumberCount-=GameStatus.MINUS_LUMBER_COUNT;
    }

    void SaveLumber(int count)
    {
        gameStatus.RemainLumberCount += count;
    }

}
