using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class CurrencySingleStackBehaviour : SimplePlayerPhysicsBehaviour, IResetable, ICurrencyHolder
{
    [Inject] private CurrencySingleStackPool singleStackPool;
    [Inject] private CurrencyPool currencyPool;

    [SerializeField] private float groundLevel = 0.4f;
    [SerializeField] private Transform moneyParent;

    private float initTime;
    private bool hasDropped;
    private CurrencyBehaviour currency;
    
    public void Initialize(int amount)
    {
        currency = currencyPool.GetElement();
        currency.transform.SetParent(moneyParent, false);
        currency.transform.localPosition = Vector3.zero;
        currency.transform.localRotation = Quaternion.identity;
        currency.Init(amount);
    }

    protected override void OnEnter(PlayerController component)
    {
        base.OnEnter(component);
        if (hasDropped)
        {
            Remove(component);   
        }
    }

    public void AddCurrency(int amount)
    {
        currency.AddAmount(amount);
    }

    public void MoveCurrencyToMe(CurrencyBehaviour currency)
    {
        
    }

    public void Remove(ICurrencyHolder target)
    {
        var retVal = currency;
        currency = null;
        singleStackPool.Pool(this);
        target.MoveCurrencyToMe(retVal);
    }

    public void OnReset()
    {
        hasDropped = true;
    }

    public void OnPool()
    {

    }

    public void AddForce(Vector3 force)
    {
        hasDropped = false;
        var start = transform.position;
        var end = start + force;
        end.y = groundLevel;
        var middle = start + end;
        middle.y = groundLevel + force.y;

        var velocity = force.magnitude;
        var timeMiddle = velocity / Physics.gravity.magnitude;
        var timeEnd = Mathf.Sqrt(2 * (end - middle).magnitude) / Physics.gravity.magnitude;
        
        transform.DOMove(middle, timeMiddle).OnComplete(() =>
        {
            transform.DOMove(end, timeEnd).OnComplete(() =>
            {
                hasDropped = false;
            }).SetEase(Ease.InCirc);
        }).SetEase(Ease.OutCirc);
    }
}
