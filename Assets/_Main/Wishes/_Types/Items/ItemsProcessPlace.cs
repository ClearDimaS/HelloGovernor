using System;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public class ItemsProcessPlace : ItemsUserProcessPlace
{
    protected ItemsWishGranter granter;

    protected override void OnAwake()
    {
        base.OnAwake();
        granter = GetComponentInParent<ItemsWishGranter>();
    }
    protected override Sprite GetItemIcon()
    {
        return granter.GetItemIcon();
    }

    public override GenericCitizenItemSource GetItemSource()
    {
        return granter.GetItemsSource();
    }
}