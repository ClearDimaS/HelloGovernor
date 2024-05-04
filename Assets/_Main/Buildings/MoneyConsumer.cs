using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public interface IMoneySpender
{
    public Transform Root { get; }
    public void Spend(int diff);
    public int MaxToSpend();
}

[Serializable]
public class Price
{
    public int price = 10;
    public int spent;
    
    public bool IsBought => spent >= price;
    
    public Price(int price, int spent)
    {
        this.price = price;
        this.spent = spent;
    }

    public Price GetCopy()
    {
        return new Price(price, spent);
    }

    public static string ToMoneyString(int money)
    {
        if (money > 1000000)
        {
            return (money / 1000000f).ToString("0.00") + "M";
        }
        else if (money > 1000)
        {
            return (money / 1000f).ToString("0.00") + "K";
        }
        else
        {
            return money.ToString();
        }
    }
}

public class MoneyConsumer : MonoBehaviour
{
    [Inject] private CurrencyPool currencyPool;
    [Inject] private GameConfig gameConfig;
    
    [SerializeField] private Transform flyTarget;
    
    private Price price;
    private IMoneySpender spender;

    private bool isReached;
    private int currentSpendAmount;
    private float currentSpendingTime;

    public event Action reachGoalEvent;
    
    public void SetPrice(Price price)
    {
        isReached = false;
        this.price = price;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.attachedRigidbody == null)
        {
            return;
        }
        
        if (other.attachedRigidbody.TryGetComponent(out IMoneySpender moneySpender) == false)
        {
            return;
        }

        SetSpender(moneySpender);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger || other.attachedRigidbody == null)
        {
            return;
        }
        
        if (other.attachedRigidbody.TryGetComponent(out IMoneySpender moneySpender) == false)
        {
            return;
        }

        if (spender == moneySpender)
        {
            spender = null;
        }
    }

    private void Update()
    {
        RefreshSpendMoney();
    }

    private void RefreshSpendMoney()
    {
        if (spender == null || price == null)
        {
            return;
        }
        if (price.IsBought)
        {
            if (!isReached)
            {
                isReached = true;
                spender = null;
                reachGoalEvent?.Invoke();
            }
            return;
        }
        currentSpendingTime += Time.deltaTime;
        
        int maxAllowedCurrentAmount = Mathf.RoundToInt(currentSpendingTime / gameConfig.moneySpendTime * price.price);

        var diff = maxAllowedCurrentAmount - currentSpendAmount;
        diff = Mathf.Min(diff, spender.MaxToSpend());
        if (diff > 0)
        {
            spender.Spend(diff);
            var currency = currencyPool.GetElement();
            currency.transform.position = spender.Root.position;
            currency.transform.DOMove(flyTarget.position, 1f).SetEase(Ease.OutCubic);
            currency.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InCubic).OnComplete(() => currencyPool.Pool(currency));
        }
        price.spent += diff;
        currentSpendAmount = maxAllowedCurrentAmount;
    }

    private void SetSpender(IMoneySpender moneySpender)
    {
        spender = moneySpender;
        currentSpendAmount = 0;
        currentSpendingTime = 0f;
    }

    public int GetSpentAmount()
    {
        if (price == null)
        {
            Debug.LogWarning($"price is null at: {transform.name}  {transform.GetInstanceID()}  {transform.parent.name}");
            return 0;
        }
        return price.spent;
    }
}