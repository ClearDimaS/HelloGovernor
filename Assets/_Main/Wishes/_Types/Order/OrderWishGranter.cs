using System;
using UnityEngine;
using Zenject;
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

public class OrderProcessPlace : ProcessPlace
{
    public override float ProcessTime { get; }
    public override bool CanAddProgress(CitizenController citizen)
    {
        throw new NotImplementedException();
    }
}

public abstract class OrderWishGranter : WishGranter<OrderWishGranterConfig, OrderProcessPlace>
{

}