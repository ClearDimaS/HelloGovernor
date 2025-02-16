using UnityEngine;
using Zenject;

public abstract class ItemsWishGranter : WishGranter<ItemsWishGranterConfig, ItemsProcessPlace>
{
    [Inject] protected PlayerController player;
    
    protected GenericCitizenItemSource itemSource;
    public int TakesCount => itemSource == null ? 0 : itemSource.TakesCount;
    public int PlayerUseCounts { get; protected set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        itemSource = GetComponentInChildren<GenericCitizenItemSource>();
    }

    public ItemsProcessPlace GetProcessedWithoutAssistant()
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
                break;
            }

            if (wishPlace.Assistant == player)
            {
                PlayerUseCounts++;
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
        foreach (var place in processPlaces)
        {
            if(place.GetOwner() != null)
            {
                return place.GetTargetTransform();
            }
        }

        return processPlaces[0].GetTargetTransform();
    }
}