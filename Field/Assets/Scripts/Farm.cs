using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_FarmOrder
{
    First,
    Second,
    Third,
    Fourth,
    Fifth,
    Sixth,
    Seventh,
}

public class Farm : MonoBehaviour
{
    [SerializeField] E_FarmOrder order;
    public E_FarmOrder Order { get { return order; } }

    public TYPE seed;
    public int remainTurn;
    public bool isWatered;
    public ItemData itemData;
    GameStatus gameStatus = null;
    ItemRoot itemRoot = null;
    Item item;
    [SerializeField] GameObject dirt;
    [SerializeField] GameObject canHarvestMarker;

    void Start()
    {
        gameStatus = GameObject.Find("GameRoot").GetComponent<GameStatus>();
        itemRoot = GameObject.Find("GameRoot").GetComponent<ItemRoot>();
        dirt.SetActive(false);
        canHarvestMarker.SetActive(false);
        isWatered = false;
        seed = TYPE.NONE;

        gameStatus.OnChangedTurn += SetRemainTurn;
    }

    public void InitFarm(ItemData data)
    {
        if (data == null)
            return;

        itemData = data;
        seed = data.ItemType;
        remainTurn = data.HarvestTerm;
        isWatered = false;

        //todo 작물 생성, 생성됐을땐 item 클래스 disabled
        GameObject go = itemRoot.RespawnItem(data.ItemType);
        go.GetComponent<BoxCollider>().enabled = false;
        go.transform.position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
        go.transform.parent = transform;
        item = go.GetComponent<Item>();

    }

    public void SetWater()
    {
        isWatered = true;
        dirt.SetActive(true);
    }

    public void SetRemainTurn(int remain)
    {
        if (remainTurn > 0 && isWatered)
        {
            remainTurn--;
            if (remainTurn == 0)
                canHarvestMarker.SetActive(true);
        }
        else
        {
            
        }
    }

    public bool isCanHarvest()
    {
        return remainTurn == 0;
    }

    public void Harvest()
    {
        TYPE type = itemData.ItemType;
        foreach (var item in gameStatus.ItemDic)
        {
            if (item.Key == type)
            {
                gameStatus.GetItem(type, 1);
                Destroy(this.item.gameObject);
                isWatered = false;
                seed = TYPE.NONE;
                remainTurn = -1;
                itemData = null;
                dirt.SetActive(false);
                canHarvestMarker.SetActive(false);
                return;
            }
        }
    }
}
