using System.Collections.Generic;
using UnityEngine;

public class OrderProcessPlace : ItemsUserProcessPlace
{
    [SerializeField] private int changeTypeAfterCount = 4;
    private int processCounts = 0;
    private OrderWishGranter granter;

    protected OrderItemsConfigData itemsConfigData;
    
    protected override void OnAwake()
    {
        base.OnAwake();
        granter = GetComponentInParent<OrderWishGranter>();
        itemsConfigData = granter.GetRandomItem();
    }

    protected override Sprite GetItemIcon()
    {
        return itemsConfigData.itemIcon;
    }

    public override bool CanAddProgress(CitizenController citizen)
    {
        return base.CanAddProgress(citizen) && wishAssistant.PeekItem() != null && 
               wishAssistant.PeekItem().GetData().key == itemsConfigData.key;
    }

    public override GenericCitizenItemSource GetItemSource()
    {
        return granter.GetItemSourceFor(itemsConfigData);
    }

    public override void LeavePlace(CitizenController citizen)
    {
        base.LeavePlace(citizen);
        processCounts++;
        if (processCounts >= changeTypeAfterCount)
        {
            processCounts = 0;
            itemsConfigData = granter.GetRandomItem();
        }
    }

    public Sprite GetCurrentIcon()
    {
        return itemsConfigData.itemIcon;
    }
    
    public Color GetCurrentColor()
    {
        return itemsConfigData.color;
    }
}