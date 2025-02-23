using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public abstract class OrderWishGranter : WishGranter<OrderWishGranterConfig, OrderProcessPlace>, IItemsUserWishGranter
{
    [SerializeField] protected Transform waiterIdlePlace;
    [SerializeField] protected GenericCitizenItemSource[] sources;
    public int PlayerUseCounts { get; protected set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        sources = GetComponentsInChildren<GenericCitizenItemSource>();
    }

    protected override void OnLeave(CitizenController citizen)
    {
        PlayerUseCounts++;
    }

    public GenericCitizenItemSource GetItemSourceFor(OrderItemsConfigData itemsConfigData)
    {
        foreach (var source in sources)
        {
            if (source.HasPrefab(itemsConfigData.prefab))
            {
                return source;
            }
        }

        return null;
    }
    
    public OrderItemsConfigData GetRandomItem()
    {
        var items = config.GetItemDatas();
        return items[Random.Range(0, items.Length)];
    }

    public float ProcessPlaceUserTime => GetTakeItemDuration();
    
    public float GetTakeItemDuration()
    {
        return config.itemTakeTime;
    }

    public ItemsUserProcessPlace GetProcessedWithoutAssistant()
    {
        foreach (var t in wishPlacesTyped)
        {
            if (t.IsAtPlace() && !t.HasAssistant())
            {
                return t;
            }
        }

        return null;
    }

    public Transform GetIdlePlace()
    {
        return waiterIdlePlace;
    }

    public Transform GetPlaceToLookAt()
    {
        return processPlaces[0].GetTargetTransform();
    }
}