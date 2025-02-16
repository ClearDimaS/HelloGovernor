using UnityEngine;

public class ItemsUseTutorialStep : TutorialStep
{
    protected float targetItemsCount = 3;
    protected ItemsWishGranter itemsGranter;
    protected int usedItemsCount;
    
    public ItemsUseTutorialStep(ItemsWishGranter itemsGranter, PlayerDataRepository repository) : base(repository)
    {
        this.itemsGranter = itemsGranter;
    }

    protected override void UpdateProgress_Internal()
    {
        usedItemsCount = itemsGranter.PlayerUseCounts;
    }

    protected override string CreateKey()
    {
        return $"items_use_{itemsGranter.GetType().Name}";
    }

    public override float GetProgress()
    {
        return usedItemsCount/targetItemsCount;
    }

    public override Transform GetCameraTarget()
    {
        return itemsGranter.GetItemUsePlace();
    }

    public override Transform GetArrowTarget()
    {
        return itemsGranter.GetItemUsePlace();
    }

    protected override string CreateProgressText()
    {
        return $"{usedItemsCount}/{Mathf.RoundToInt(targetItemsCount)}";
    }

    protected override string CreateTitle()
    {
        return $"Use {Mathf.RoundToInt(targetItemsCount)} items";
    }

    public override Sprite GetTutorialIcon()
    {
        return itemsGranter.GetItemIcon();
    }
}