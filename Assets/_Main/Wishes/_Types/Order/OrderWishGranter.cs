using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public abstract class OrderWishGranter : WishGranter<OrderWishGranterConfig, OrderProcessPlace>, IItemsUserWishGranter
{
    public int PlayerUseCounts { get; protected set; }

    protected override void OnLeave(CitizenController citizen)
    {
        PlayerUseCounts++;
    }

    public OrderItemsConfigData GetRandomItem()
    {
        var items = config.GetItemDatas();
        return items[Random.Range(0, items.Length)];
    }

    public float GetTakeItemDuration()
    {
        return config.itemTakeTime;
    }
}