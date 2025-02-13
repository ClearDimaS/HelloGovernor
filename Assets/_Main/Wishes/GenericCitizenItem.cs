using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New Item")]
public class ItemConfigData : ScriptableObject
{
    public string key = "";
}

[Serializable]
public class ItemsPlacesData
{
    public Transform[] places;

    public Transform GetPlace(int index)
    {
        return places[index];
    }
}

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
        var poolTmp = pool;
        pool = null;
        poolTmp.Pool(this);
    }

    public void SetPool(GenericItemsPool pool)
    {
        this.pool = pool;
    }
}