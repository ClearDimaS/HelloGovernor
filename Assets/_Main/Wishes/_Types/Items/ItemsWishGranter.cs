using UnityEngine;
using Zenject;

public abstract class ItemsWishGranter : WishGranter<ItemsWishGranterConfig, ItemsProcessPlace>, IItemsUserWishGranter
{
    [Inject] protected PlayerController player;
    
    protected GenericCitizenItemSource itemSource;
    public int TakesCount => itemSource == null ? 0 : itemSource.TakesCount;
    public int PlayerUseCounts { get; protected set; }

    [field: SerializeField] public float ProcessPlaceUserTime { get; private set; } = 1f;
    
    public Transform GetIdlePlace()
    {
        return itemSource.IdlePlace;
    }

    public Transform GetPlaceToLookAt()
    {
        return processPlaces[0].GetTargetTransform();
    }
    
    protected override void OnAwake()
    {
        base.OnAwake();
        itemSource = GetComponentInChildren<GenericCitizenItemSource>(true);
    }

    public ItemsUserProcessPlace GetProcessedWithoutAssistant()
    {
        foreach (var place in wishPlacesTyped)
        {
            if (!place.HasAssistant() && place.GetOwner() != null)
            {
                return place;
            }
        }

        return null;
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
    
    public Sprite GetItemIcon()
    {
        return config.GetItemsData().itemIcon;
    }

    public float GetTakeItemDuration()
    {
        return config.GetItemsData().itemTakeTime;
    }

    public Sprite GetIconAssistant()
    {
        return config.GetItemsData().assistantIcon;
    }

    public GenericCitizenItem[] GetPrefabs()
    {
        return config.GetItemsData().prefabs;
    }

    public Transform GetItemTakePlace()
    {
        return itemSource.TakePlace;
    }
    
    public Transform GetItemUsePlace()
    {
        foreach (var place in wishPlacesTyped)
        {
            if(place.GetOwner() != null)
            {
                return place.GetItemSpendPlace();
            }
        }

        return wishPlacesTyped[0].GetItemSpendPlace();
    }

    public GenericCitizenItemSource GetItemsSource()
    {
        return itemSource;
    }
}