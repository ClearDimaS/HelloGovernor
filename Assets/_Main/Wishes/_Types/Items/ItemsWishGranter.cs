using UnityEngine;

public abstract class ItemsWishGranter : WishGranter<ItemsWishGranterConfig, ItemsProcessPlace>
{
    protected override bool CanAddProgress(CitizenController citizen)
    {
        return citizen.WishesController.IsGranterAssistantServing(this);
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