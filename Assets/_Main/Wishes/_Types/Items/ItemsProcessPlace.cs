using System;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public class ItemsProcessPlace : ItemsUserProcessPlace
{
    [SerializeField] protected GameObject gfx;
    protected ItemsWishGranter granter;

    private void OnDisable()
    {
        if (gfx != null)
        {
            gfx.SetActive(false);
        }
    }

    private void OnEnable()
    {
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