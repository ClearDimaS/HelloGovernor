using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Wishes/OrderGranter", fileName = "OrderGranterConfig")]
public class OrderWishGranterConfig : WishGranterConfig
{
    [SerializeField] private OrderItemsConfigData itemsData;
    public Sprite assistantIcon;
    public float itemTakeTime;

    public OrderItemsConfigData GetItemsData()
    {
        return itemsData;
    }
}

[Serializable]
public class OrderItemsConfigData
{
    public string key;
    public Sprite itemIcon;
    [SerializeField] private GenericCitizenItem _prefab;
    public GenericCitizenItem prefab => _prefab;
}
