using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

public interface IThiefBuster : IRootProvider
{
    public void ReturnMoney(int stolenAmount, Transform transform);
}

[Serializable]
public class ThiefSpawnData
{
    public Transform spawn;
    public Transform escape;
}

public class ThiefBehaviour : SimpleThiefBusterPhysicsBehaviour, ICurrencyHolder, IResetable
{
    [Inject] protected ThiefsManager thiefsManager;
    [Inject] protected CurrencyPool currencyPool;
    [Inject] protected GameConfig config;
    
    [SerializeField] protected Walker walker;

    protected Transform escapePoint;
    protected CurrencyStackBehaviour target;

    protected bool isBusted;
    public bool IsStealing => stolenAmount > 0;
    public bool IsBusted => isBusted;

    protected int stolenAmount;

    protected override void OnEnter(IThiefBuster thiefBuster)
    {
        base.OnEnter(thiefBuster);
        if (stolenAmount > 0)
        {
            thiefBuster.ReturnMoney(stolenAmount, transform);
            thiefsManager.StopSteal(this);
            stolenAmount = 0;
            isBusted = true;
        }
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (stolenAmount < config.thiefMaxSteal && !isBusted)
        {
            if (target == null || target.GetMoney() <= 0)
            {
                target = thiefsManager.GetThiefTarget();
            }
            else
            {
                walker.MoveToTarget(target.transform.position, TakeMoney);  
            }   
        }
        else
        {
            walker.MoveToTarget(escapePoint.position, () =>
            {
                thiefsManager.FinishSteal(this);
            });
        }
    }

    public void Init(CurrencyStackBehaviour stack, Transform escapePoint)
    {
        this.escapePoint = escapePoint;
    }

    private void TakeMoney()
    {
        if (target != null)
        {
            if (stolenAmount < config.thiefMaxSteal)
            {
                target.RemoveOne(this);
            }
        }
    }

    public void AddCurrency(int amount)
    {

    }

    public void MoveCurrencyToMe(CurrencyBehaviour currency)
    {
        stolenAmount += currency.Amount;
        
        currency.transform.DOMove(transform.position, 0.3f);
        currency.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
        {
            currencyPool.Pool(currency);
        });
    }

    public void OnReset()
    {
        target = null;
        stolenAmount = 0;
        isBusted = false;
    }

    public void OnPool()
    {

    }
}
