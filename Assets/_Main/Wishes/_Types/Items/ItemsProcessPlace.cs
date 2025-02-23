using System;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public class ItemsProcessPlace : ItemsUserProcessPlace
{
    protected override GenericCitizenItem GetItemPrefab()
    {
        throw new NotImplementedException();
    }

    protected override Sprite GetItemIcon()
    {
        throw new NotImplementedException();
    }

    public override GenericCitizenItemSource GetItemSource()
    {
        throw new NotImplementedException();
    }

    public override Transform GetItemTakePlace()
    {
        throw new NotImplementedException();
    }

    public override Transform GetItemSpendPlace()
    {
        throw new NotImplementedException();
    }
}