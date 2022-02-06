using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameStatus : MonoBehaviour
{
    // 철광석, 식물을 사용했을 때 각각의 수리 정도.
    public static float GAIN_REPAIRMENT_IRON = 0.30f;
    public static float GAIN_REPAIRMENT_PLANT = 0.10f;
    // 철광석, 사과, 식물을 운반했을 때 각각의 체력 소모 정도.
    public static float CONSUME_SATIETY_IRON = 0.15f;
    public static float CONSUME_SATIETY_APPLE = 0.1f;
    public static float CONSUME_SATIETY_PLANT = 0.1f;

    // 사과, 식물을 먹었을 때 각각의 체력 회복 정도.
    public static float REGAIN_SATIETY_APPLE = 0.7f;
    public static float REGAIN_SATIETY_PLANT = 0.2f;
    public float repairment = 0.0f; // 우주선의 수리 정도(0.0f~1.0f).
    public float satiety = 1.0f; // 배고픔,체력(0.0f~1.0f).
    public GUIStyle guistyle; // 폰트 스타일.

    public static float CONSUME_SATIETY_ALWAYS = 0.03f;
    public const int INVENTORY_MAX_COUNT = 13;
    public static int MAX_REMAIN_TURN = 10; // 턴
    public static int MINUS_LUMBER_COUNT = 4;   // 소비 나무

    // 인벤토리
    /// <summary>
    /// </summary>
    public Dictionary<TYPE, int> ItemDic = new Dictionary<TYPE, int>();

    // 도구
    public Dictionary<int, ToolData> toolDic = new Dictionary<int, ToolData>();

    public Action<Dictionary<TYPE, int>> OnUpdatedItemList { get; set; }
    public Action<TYPE> OnClickedSeedBtn { get; set; }
    public Action<int> OnChangedTurn { get; set; }
    public Action<int, int, int, int> OnUpdateGameInfo { get; set; }

    private int penalty = 0;
    public int Penalty
    {
        get { return penalty; }
        set
        {
            if (value == penalty)
                return;

            penalty = value;
            OnUpdateGameInfo(Hp, Gold, RemainLumberCount, Penalty);
        }
    }
    
    private int remainLumberCount = 0;
    public int RemainLumberCount
    {
        get { return remainLumberCount; }
        set
        {
            if (value == remainLumberCount)
                return;

            remainLumberCount = value;
            OnUpdateGameInfo(Hp, Gold, RemainLumberCount, Penalty);
        }
    }
    private int gold = 0;
    public int Gold
    {
        get { return gold; }
        set
        {
            if (value == gold)
                return;

            gold = value;
            OnUpdateGameInfo(Hp, Gold, RemainLumberCount, Penalty);
        }
    }
    private int hp = 100;
    public int Hp
    {
        get { return hp; }
        set
        {
            if (value == hp)
                return;

            hp = value;
            OnUpdateGameInfo(Hp, Gold, RemainLumberCount, Penalty);
        }
    }
    public int RemainTurn = MAX_REMAIN_TURN;

    // 배를 고프게 하는 메서드 추가
    public void alwaysSatiety()
    {
        this.satiety = Mathf.Clamp01(this.satiety - CONSUME_SATIETY_ALWAYS * Time.deltaTime);
    }

    private void InitToolList()
    {
        ToolData axe = new ToolData(TYPE.Axe);
        ToolData springkler = new ToolData(TYPE.Springkler);

        toolDic.Add(0, axe);
        toolDic.Add(1, springkler);
    }

    private void InitItemList()
    {
        ItemDic.Add(TYPE.Lumber, 0);
        ItemDic.Add(TYPE.Macintosh, 0);
        ItemDic.Add(TYPE.Corn, 0);
        ItemDic.Add(TYPE.Orange, 0);
        ItemDic.Add(TYPE.Tomato, 0);
        ItemDic.Add(TYPE.Grape, 0);
        ItemDic.Add(TYPE.Strawberry, 0);
        ItemDic.Add(TYPE.MSeed, 0);
        ItemDic.Add(TYPE.CSeed, 0);
        ItemDic.Add(TYPE.OSeed, 0);
        ItemDic.Add(TYPE.TSeed, 0);
        ItemDic.Add(TYPE.GSeed, 0);
        ItemDic.Add(TYPE.SSeed, 0);
    }

    void Start()
    {
        InitToolList();
        InitItemList();

        if (OnUpdatedItemList != null)
            OnUpdatedItemList(ItemDic);

        this.guistyle.fontSize = 24; // 폰트 크기를 24로.
        OnUpdateGameInfo(Hp, Gold, RemainLumberCount, penalty);
    }

    // 체력을 늘리거나 줄임
    public void addSatiety(float add)
    {
        this.satiety = Mathf.Clamp01(this.satiety + add);
    }

    // 게임을 클리어했는지 검사
    public bool isGameClear()
    {
        bool is_clear = false;
        if (this.repairment >= 1.0f)
        { // 수리 정도가 100% 이상이면.
            is_clear = true; // 클리어했다.
        }
        return (is_clear);
    }

    // 게임이 끝났는지 검사
    public bool isGameOver()
    {
        bool is_over = false;
        if (RemainTurn <= 0)
        { // 체력이 0이하라면.
            is_over = true; // 게임 오버.
        }
        return (is_over);
    }

    public void SellAllItem()
    {
        int sum = 0;
        List<TYPE> removeList = new List<TYPE>();
        foreach (var item in ItemDic)
        {
            if (item.Value == 0)
                continue;

            switch (item.Key)
            {
                case TYPE.Macintosh:    //10
                    sum += 10 * item.Value;
                    removeList.Add(item.Key);
                    break;

                case TYPE.Corn:     //5
                    sum += 5 * item.Value;
                    removeList.Add(item.Key);
                    break;

                case TYPE.Orange:   //10
                    sum += 10 * item.Value;
                    removeList.Add(item.Key);
                    break;

                case TYPE.Tomato:   //15
                    sum += 15 * item.Value;
                    removeList.Add(item.Key);
                    break;

                case TYPE.Grape:    //11
                    sum += 11 * item.Value;
                    removeList.Add(item.Key);
                    break;

                case TYPE.Strawberry:   //12
                    sum += 12 * item.Value;
                    removeList.Add(item.Key);
                    break;

                default:
                    continue;
            }

            //ItemDic[item.Key] = 0;

        }

        for (int i = 0; i < removeList.Count; i++)
        {
            ItemDic[removeList[i]] = 0;
        }

        Gold += sum;
        OnUpdatedItemList(ItemDic);
    }

    public void GetItem(TYPE type, int count)
    {
        foreach (var item in ItemDic)
        {
            if (item.Key == type)
            {
                ItemDic[item.Key] += count;
                switch (item.Key)
                {
                    case TYPE.Macintosh:
                        ItemDic[TYPE.MSeed] += 1;
                        ItemDic[TYPE.TSeed] += 1;
                        break;

                    case TYPE.Corn:
                        ItemDic[TYPE.CSeed] += 1;
                        ItemDic[TYPE.GSeed] += 1;
                        break;

                    case TYPE.Orange:
                        ItemDic[TYPE.OSeed] += 1;
                        ItemDic[TYPE.SSeed] += 1;
                        break;

                    case TYPE.Tomato:
                        ItemDic[TYPE.TSeed] += 1;
                        break;
                    case TYPE.Grape:
                        ItemDic[TYPE.GSeed] += 1;
                        break;
                    case TYPE.Strawberry:
                        ItemDic[TYPE.SSeed] += 1;
                        break;
                }


                OnUpdatedItemList(ItemDic);
                return;
            }
        }
    }

    public void ChangeTurn()
    {
        RemainTurn--;

        if (OnChangedTurn != null)
            OnChangedTurn(RemainTurn);
    }

    public bool IsCanWork(int useHP)
    {
        if (Hp - useHP < 0)
            return false;

        return true;
    }
}
