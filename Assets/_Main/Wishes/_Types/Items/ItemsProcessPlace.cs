using System;
using UnityEngine;

public class ItemsProcessPlace : ProcessPlace
{
    [field: SerializeField] public Transform ItemTakePlace { get; private set; }
    [SerializeField] private TimerBase takeTimer;
    
    private ItemsWishGranter itemsWishGranter;

    public override float ProcessTime => itemsWishGranter.FullProgressTime;
    
    private void Start()
    {
        itemsWishGranter = GetComponentInParent<ItemsWishGranter>();
        takeTimer.SetIcon(itemsWishGranter.GetItemIcon());
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        takeTimer.SetProgress(progress);
    }

    public override bool CanAddProgress(CitizenController citizen)
    {
        return citizen.WishesController.IsGranterAssistantServing(itemsWishGranter);
    }
}