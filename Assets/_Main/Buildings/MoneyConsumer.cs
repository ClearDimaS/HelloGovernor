using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IMoneySpender
{
    
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
}

public class MoneyConsumer : MonoBehaviour
{
    [Inject] private GameConfig gameConfig;
    
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
        price.spent += maxAllowedCurrentAmount - currentSpendAmount;
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