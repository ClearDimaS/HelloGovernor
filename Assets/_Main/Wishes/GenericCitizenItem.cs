using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class GenericCitizenItem : CitizenItem, IResetable
{
    [SerializeField] private ItemConfigData itemsData;

    private int resetCounter;
    private GenericItemsPool pool;
    private Action onPool;
    public string secondaryKey;

    public void OnReset()
    {
        resetCounter++;
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

        var fire = onPool;
        fire?.Invoke();
        onPool = null;
        
        var poolTmp = pool;
        pool = null;
        poolTmp.Pool(this);
    }

    public override void PoolPleaseAtTimeout(Action action)
    {
        var cntr = resetCounter;
        onPool = action;
        UniTask.Delay(TimeSpan.FromSeconds(itemsData.timeOut)).ContinueWith(() =>
        {
            if (resetCounter == cntr)
            {
                PoolPlease();   
            }
        });
    }

    public void SetPool(GenericItemsPool pool)
    {
        this.pool = pool;
    }
}