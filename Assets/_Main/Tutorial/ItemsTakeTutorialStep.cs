using System;
using UnityEngine;

public class ItemsTakeTutorialStep : TutorialStep
{
    protected float targetItemsCount = 3;
    protected IItemsUserWishGranter itemsGranter;
    protected int takenItemsCount;
    
    public ItemsTakeTutorialStep(IItemsUserWishGranter itemsGranter, PlayerDataRepository repository) : base(repository)
    {
        this.itemsGranter = itemsGranter;
    }

    protected override string CreateKey()
    {
        return $"items_take_{itemsGranter.GetType().Name}";
    }

    protected override void UpdateProgress_Internal()
    {
        takenItemsCount = itemsGranter.TakesCount;
    }

    public override float GetProgress()
    {
        return takenItemsCount/targetItemsCount;
    }

    public override Transform GetCameraTarget()
    {
        return itemsGranter.GetItemTakePlace();
    }

    public override Transform GetArrowTarget()
    {
        return itemsGranter.GetItemTakePlace();
    }

    protected override string CreateProgressText()
    {
        return $"{takenItemsCount}/{Mathf.RoundToInt(targetItemsCount)}";
    }

    protected override string CreateTitle()
    {
        return $"Take {Mathf.RoundToInt(targetItemsCount)} items";
    }

    public override Sprite GetTutorialIcon()
    {
        return itemsGranter.GetItemIcon();
    }
}