using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeMenu : MonoBehaviour
{
    static CustomizeMenu instance;

    public Character character;

    public Transform scroll;
    public AccessoryItemBtn itemBtn;

    List<AccessoryItemBtn> buttons = new List<AccessoryItemBtn>();

    private void Start()
    {
        for (int i = 0; i < scroll.childCount; i++)
        {
            Destroy(scroll.GetChild(0).gameObject);
        }

        foreach(var item in Character.AccessoryItems)
        {
            AccessoryItemBtn btn = Instantiate(itemBtn, scroll);
            buttons.Add(btn);
            btn.Init(item);
        }
    }

    public static bool DressItem(AccessoryItem item)
    {
        return instance.character.DressOrUndress(item);
    }

    public static void RefreshMenu()
    {
        foreach (AccessoryItemBtn btn in instance.buttons)
        {
            btn.Dressed = instance.character.IsItemDressed(btn.item);
        }
    }

    private void Awake()
    {
        instance = this;
    }
}
