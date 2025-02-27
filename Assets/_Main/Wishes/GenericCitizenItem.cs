using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

    public override void PoolPleaseAtTimeout(Action action)
    {
        UniTask.Delay(TimeSpan.FromSeconds(itemsData.timeOut)).ContinueWith(() =>
        {
            PoolPlease();
        });
    }

    public void SetPool(GenericItemsPool pool)
    {
        this.pool = pool;
    }
}