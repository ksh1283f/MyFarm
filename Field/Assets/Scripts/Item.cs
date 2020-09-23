using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] TYPE type;
    [SerializeField] E_Kind kind;

    public ItemData itemData;
    private void Awake()
    {
        itemData = new ItemData(type, kind);
    }
}