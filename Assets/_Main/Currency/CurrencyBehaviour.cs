using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyBehaviour : MonoBehaviour, IResetable, IGridPlaceable
{
    private int amount;

    public Transform Root => transform;
    public int Amount => amount;
    
    public void Init(int amount)
    {
        this.amount = amount;
    }

    public void AddAmount(int amount)
    {
        this.amount += amount;
    }

    public void OnReset()
    {
        transform.SetParent(null);
        transform.localScale = Vector3.one;
    }

    public void OnPool()
    {
     
    }

    public Vector3 GetWorldSize()
    {
        return Vector3.one;
    }
}
