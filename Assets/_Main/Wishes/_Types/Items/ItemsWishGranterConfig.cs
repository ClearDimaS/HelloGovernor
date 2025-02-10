using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Wishes/ItemsGranter", fileName = "ItemsGranterConfig")]
public class ItemsWishGranterConfig : WishGranterConfig
{
    private ItemsConfigData itemsData;

    public ItemsConfigData GetItemsData()
    {
        return itemsData;
    }
}

[Serializable]
public class ItemsConfigData
{
    public GameObject assistantPrefab;
    public Sprite assistantIcon;
    
    public Sprite itemIcon;
    public float itemTakeTime;
}
