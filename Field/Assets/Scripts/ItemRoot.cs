using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TYPE
{ // 아이템 종류.
    NONE = -1, // 없음.
    IRON = 0, // 철광석.
    APPLE, // 사과.
    PLANT, // 식물.
    NUM, // 아이템이 몇 종류인가 나타낸다(=3).

    // 인게임 아이템
    Lumber, // 나무
    Macintosh,  // 사과
    Corn,   // 옥수수
    Orange, // 오렌지

    Tomato, // 토마토
    Grape,  // 포토
    Strawberry, // 딸기

    // 씨앗
    MSeed,  // 사과씨앗
    CSeed,   // 옥수수씨앗
    OSeed, // 오렌지씨앗

    TSeed, // 토마토씨앗
    GSeed,  // 포토씨앗
    SSeed, // 딸기씨앗

    // 도구
    Axe,    // 도끼
    Springkler,  // 물뿌리개
};

/// <summary>
/// 아이템 종류
/// </summary>
public enum E_Kind
{
    None,
    A,
    B,
    C,
}

/// <summary>
/// 인게임 아이템등급
/// </summary>
public enum E_Grade
{
    Upper,
    Lower,
}

public class ItemData
{
    public TYPE ItemType { get; set; }
    public E_Kind ItemKind { get; set; }
    public E_Grade ItemGrade { get; set; }

    public int HarvestTerm { get; set; }    // 수확기간 
    public int ItemValue { get; set; }  // 판매 가격
    public int HarvestValue { get; set; }   // 수확량
    public int UseHP { get; set; }  // 사용 체력

    public ItemData(TYPE type, E_Kind kind)
    {
        ItemType = type;
        ItemKind = kind;

        switch (ItemType)
        {
            case TYPE.Macintosh:
            case TYPE.Corn:
            case TYPE.Orange:
                ItemValue = 50;
                UseHP = 10;
                break;

            case TYPE.Tomato:
            case TYPE.Grape:
            case TYPE.Strawberry:
                ItemValue = 100;
                UseHP = 30;
                break;

            case TYPE.Axe:
            case TYPE.Springkler:
                UseHP = 10;
                break;

            case TYPE.MSeed:
            case TYPE.CSeed:
            case TYPE.OSeed:
                UseHP = 10;
                break;

            case TYPE.TSeed:
            case TYPE.GSeed:
            case TYPE.SSeed:
                UseHP = 30;
                break;
        }

        switch (ItemKind)
        {
            case E_Kind.A:
                HarvestTerm = 1;
                HarvestValue = 1;
                break;
            case E_Kind.B:
                HarvestTerm = 4;
                HarvestValue = 4;
                break;
            case E_Kind.C:
                HarvestTerm = 2;
                HarvestValue = 1;
                break;
        }
    }
};

public class ToolData
{
    public TYPE ItemType { get; set; }

    public ToolData(TYPE type)
    {
        ItemType = type;
    }
}

public class ItemRoot : MonoBehaviour
{
    public GameObject ironPrefab = null; // Prefab 'Iron'
    public GameObject plantPrefab = null; // Prefab 'Plant'
    public GameObject applePrefab = null; // Prefab 'Apple'

    public GameObject Lumber = null;
    public GameObject Macintosh = null;
    public GameObject Corn = null;
    public GameObject Orange = null;
    public GameObject Grape = null;
    public GameObject Tomato = null;
    public GameObject Strawberry = null;
    public GameObject Dirty = null;

    public GameObject ItemRespawn = null;
    public GameStatus gameStatus = null;

    public float step_timer = 0.0f;
    public static float RESPAWN_TIME_LUMBER = 3f; // 나무 출현 시간 상수.
    private float respawnTimerLumber = 0.0f; // 식물의 출현 시간

    public int MaxLumberCount = 10;
    public int MaxItemCount = 5;
    public List<GameObject> respawnedItemList = new List<GameObject>();

    // 아이템의 종류를 Item.TYPE형으로 반환하는 메소드.
    public TYPE getItemType(GameObject item_go)
    {
        TYPE type = TYPE.NONE;
        if (item_go != null)
        {
            // 인수로 받은 GameObject가 비어있지 않으면.
            Item item = item_go.GetComponent<Item>();
            if (item != null)
                return item.itemData.ItemType;

            //switch (item_go.tag)
            //{ // 태그로 분기.
            //    case "Iron": type = TYPE.IRON; break;
            //    case "Apple": type = TYPE.APPLE; break;
            //    case "Plant": type = TYPE.PLANT; break;
            //}
        }
        return (type);
    }

    // 아이템을 출현시킨다.
    public GameObject RespawnItem(TYPE itemType)
    {
        GameObject item = null;
        switch (itemType)
        {
            case TYPE.Lumber:
                item = Lumber;
                break;
            case TYPE.Macintosh:
                item = Macintosh;
                break;
            case TYPE.Corn:
                item = Corn;
                break;
            case TYPE.Orange:
                item = Orange;
                break;
            case TYPE.Tomato:
                item = Tomato;
                break;
            case TYPE.Grape:
                item = Grape;
                break;
            case TYPE.Strawberry:
                item = Strawberry;
                break;

            default:
                Debug.LogError("Invalid type: " + itemType.ToString());
                return null;
        }

        // 철광석 프리팹을 인스턴스화.
        if (item == null)
        {
            Debug.LogError("item is null");
            return null;
        }

        GameObject go = GameObject.Instantiate(item) as GameObject;
        Vector3 pos = ItemRespawn.transform.position;
        // 출현 위치를 조정.
        pos.y = 1.0f;
        pos.x += Random.Range(-10.0f, 10.0f);
        pos.z += Random.Range(-10.0f, 10.0f);
        // 철광석의 위치를 이동.
        go.transform.position = pos;
        return go;
    }

    // 각 아이템의 타이머 값이 출현 시간을 초과하면 해당 아이템을 출현.
    IEnumerator Start()
    {
        gameStatus = GetComponent<GameStatus>();
        while (true)
        {
            if (gameStatus.isGameOver())
                yield break;

            yield return new WaitForSeconds(1f);
            if (respawnedItemList.Count <= GetLimitCount())
            {
                respawnTimerLumber++;
                if (respawnTimerLumber > RESPAWN_TIME_LUMBER)
                {
                    respawnTimerLumber = 0f;
                    GameObject go = RespawnItem(TYPE.Lumber);
                    respawnedItemList.Add(go);
                    continue;
                }

                int itemRespawnPercet = Random.Range(0, 100);
                if (itemRespawnPercet < 10)
                {
                    int item = Random.Range(0, 2);
                    GameObject go = null;
                    if (item == 0) go = RespawnItem(TYPE.Macintosh);
                    else if (item == 1) go = RespawnItem(TYPE.Corn);
                    else if (item == 2) go = RespawnItem(TYPE.Orange);
                    if (go != null)
                        respawnedItemList.Add(go);
                }
            }
        }
    }

    int GetLimitCount()
    {
        int count = -1;
        if (gameStatus.RemainTurn >= 7 || gameStatus.RemainTurn <= 10)
            count = 20;
        else if (gameStatus.RemainTurn >= 3 || gameStatus.RemainTurn <= 6)
            count = 10;
        else
            count = 5;

        return count;
    }   



    // 들고 있는 아이템에 따른 ‘수리 진척 상태’를 반환
    public float getGainRepairment(GameObject item_go)
    {
        float gain = 0.0f;
        if (item_go == null)
        {
            gain = 0.0f;
        }
        else
        {
            TYPE type = this.getItemType(item_go);
            switch (type)
            { // 들고 있는 아이템의 종류로 갈라진다.
                case TYPE.IRON:
                    gain = GameStatus.GAIN_REPAIRMENT_IRON; break;
                case TYPE.PLANT:
                    gain = GameStatus.GAIN_REPAIRMENT_PLANT; break;
            }
        }
        return (gain);
    }

    // 들고 있는 아이템에 따른 ‘체력 감소 상태’를 반환
    public float getConsumeSatiety(GameObject item_go)
    {
        float consume = 0.0f;
        if (item_go == null)
        {
            consume = 0.0f;
        }
        else
        {
            TYPE type = this.getItemType(item_go);
            switch (type)
            { // 들고 있는 아이템의 종류로 갈라진다.
                case TYPE.IRON:
                    consume = GameStatus.CONSUME_SATIETY_IRON; break;
                case TYPE.APPLE:
                    consume = GameStatus.CONSUME_SATIETY_APPLE; break;
                case TYPE.PLANT:
                    consume = GameStatus.CONSUME_SATIETY_PLANT; break;
            }
        }
        return (consume);
    }

    // 들고 있는 아이템에 따른 ‘체력 회복 상태’를 반환
    public float getRegainSatiety(GameObject item_go)
    {
        float regain = 0.0f;
        if (item_go == null)
        {
            regain = 0.0f;
        }
        else
        {
            TYPE type = this.getItemType(item_go);
            switch (type)
            { // 들고 있는 아이템의 종류로 갈라진다.
                case TYPE.APPLE:
                    regain = GameStatus.REGAIN_SATIETY_APPLE; break;
                case TYPE.PLANT:
                    regain = GameStatus.REGAIN_SATIETY_PLANT; break;
            }
        }
        return (regain);
    }


}

