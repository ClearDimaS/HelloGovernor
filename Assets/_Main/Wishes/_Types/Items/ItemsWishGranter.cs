using UnityEngine;

public abstract class ItemsWishGranter : WishGranter<ItemsWishGranterConfig, ItemsProcessPlace>
{
    public CitizenController GetProcessedWithoutAssistant()
    {
        foreach (var citizen in processed)
        {
            if (!citizen.WishesController.IsGranterAssistantServing(this))
            {
                return citizen;
            }
        }

        return null;
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
}