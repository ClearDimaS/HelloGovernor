using System;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public class ItemsProcessPlace : ItemsUserProcessPlace
{
    [SerializeField] protected GameObject gfx;
    protected ItemsWishGranter granter;

    protected override void OnOnDisable()
    {
        base.OnOnDisable();
        if (gfx != null)
        {
            gfx.SetActive(false);
        }
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();
        if (gfx != null)
        {
            gfx.SetActive(true);
        }
    }

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