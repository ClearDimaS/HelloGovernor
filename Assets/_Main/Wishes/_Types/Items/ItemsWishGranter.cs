using UnityEngine;

public abstract class ItemsWishGranter : WishGranter<ItemsWishGranterConfig, ItemsProcessPlace>
{
    public ItemsProcessPlace GetProcessedWithoutAssistant()
    {
        foreach (var place in wishPlacesTyped)
        {
            if (!place.HasAssistant())
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

    public WishAssistantItem[] GetPrefabs()
    {
        return config.GetItemsData().prefabs;
    }
}