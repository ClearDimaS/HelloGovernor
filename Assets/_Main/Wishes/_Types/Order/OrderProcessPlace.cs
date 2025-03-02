using System.Collections.Generic;
using UnityEngine;

public class OrderProcessPlace : ItemsUserProcessPlace
{
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

    public override GenericCitizenItemSource GetItemSource()
    {
        return granter.GetItemSourceFor(itemsConfigData);
    }

    public override void LeavePlace(CitizenController citizen)
    {
        base.LeavePlace(citizen);
        itemsConfigData = granter.GetRandomItem();
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