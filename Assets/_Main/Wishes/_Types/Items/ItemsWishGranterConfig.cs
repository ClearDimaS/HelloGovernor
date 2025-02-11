using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Wishes/ItemsGranter", fileName = "ItemsGranterConfig")]
public class ItemsWishGranterConfig : WishGranterConfig
{
    [SerializeField] private ItemsConfigData itemsData;

    public ItemsConfigData GetItemsData()
    {
        return itemsData;
    }
}

[Serializable]
public class ItemsConfigData
{
    public Sprite assistantIcon;
    
    public Sprite itemIcon;
    public float itemTakeTime;
    public WishAssistantItem[] prefabs;
}
