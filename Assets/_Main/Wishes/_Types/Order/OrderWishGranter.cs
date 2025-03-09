using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public abstract class OrderWishGranter : WishGranter<OrderWishGranterConfig, OrderProcessPlace>, IItemsUserWishGranter
{
    [Inject] protected PlayerController player;
    
    [SerializeField] protected Transform waiterIdlePlace;
    [SerializeField] protected GenericCitizenItemSource[] sources;
    public int PlayerUseCounts { get; protected set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        sources = GetComponentsInChildren<GenericCitizenItemSource>();
        foreach (var source in sources)
        {
            foreach (var data in config.GetItemDatas())
            {
                if (source.HasPrefab(data.prefab))
                {
                    source.SetItemIcon(data.itemIcon, data.color);
                }
            }
        }
    }

    protected override void OnLeave(CitizenController citizen)
    {
        foreach (var wishPlace in wishPlacesTyped)
        {
            if (wishPlace.GetOwner() == citizen)
            {
                var item = wishPlace.RemoveItem();
                citizen.AddItem(item);
                if (wishPlace.Assistant == player)
                {
                    PlayerUseCounts++;
                }
                break;
            }
        }
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

    public Sprite GetIconAssistant()
    {
        return config.assistantIcon;
    }
}