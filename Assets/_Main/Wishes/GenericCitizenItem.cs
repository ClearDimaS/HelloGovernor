using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ItemsData
{
    public string key = "";
    public Vector3[] itemLocalPlaces;
    public HumanBodyBones bone;
    public Vector3 rootLocalPlace;
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
    [SerializeField] private ItemsData itemsData;
    
    private GenericItemsPool pool;

    public void OnReset()
    {
        
    }

    public void OnPool()
    {

    }

    public override ItemsData GetData()
    {
        return itemsData;
    }

    public override ItemsPlacesData CreatePlaces(Animator animator)
    {
        return new ItemsPlacesData();
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