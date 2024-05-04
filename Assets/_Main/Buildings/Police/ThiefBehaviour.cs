using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

public interface IThiefBuster
{
    
}

[Serializable]
public class ThiefSpawnData
{
    public Transform spawn;
    public Transform escape;
}

public class ThiefBehaviour : MonoBehaviour, ICurrencyHolder, IResetable
{
    [Inject] protected ThiefsManager thiefsManager;
    [Inject] protected CurrencyPool currencyPool;
    [Inject] protected PlayerDataRepository repository;
    [Inject] protected GameConfig config;
    
    [SerializeField] protected Walker walker;

    protected Transform escapePoint;
    protected CurrencyStackBehaviour target;
    protected List<CurrencyStackBehaviour> stacks = new ();

    protected bool isBusted;
    public bool IsStealing => stolenAmount > 0;
    protected int stolenAmount;
    
    private void Awake()
    {
        stacks = FindObjectsOfType<CurrencyStackBehaviour>(true).ToList();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.attachedRigidbody == null)
        {
            return;
        }

        if (!other.attachedRigidbody.TryGetComponent(out IThiefBuster thiefBuster))
        {
            return;
        }

        if (stolenAmount > 0)
        {
            repository.GetData().money += stolenAmount;
            stolenAmount = 0;
            isBusted = true;
        }
    }

    private void Update()
    {
        if (stolenAmount < config.thiefMaxSteal && !isBusted)
        {
            if (target == null || target.Money <= 0)
            {
                RefreshTarget();
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
    
    public void Init(Transform escapePoint)
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

    private void RefreshTarget()
    {
        for (int i = 0; i < stacks.Count; i++)
        {
            if (stacks[i].Money > config.moneyToGather)
            {
                target = stacks[i];
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
