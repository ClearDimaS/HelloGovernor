using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GenericCitizenItem : CitizenItem, IResetable
{
    [SerializeField] private ItemConfigData itemsData;
    
    private GenericItemsPool pool;

    public void OnReset()
    {
        
    }

    public void OnPool()
    {

    }

    public override ItemConfigData GetData()
    {
        return itemsData;
    }

    public override void PoolPlease()
    {
        if (pool == null)
        {
            return;
        }
        var poolTmp = pool;
        pool = null;
        poolTmp.Pool(this);
    }

    public void SetPool(GenericItemsPool pool)
    {
        this.pool = pool;
    }
}