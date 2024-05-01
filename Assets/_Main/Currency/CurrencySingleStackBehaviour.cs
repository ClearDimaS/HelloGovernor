using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CurrencySingleStackPool : MonoPool<CurrencySingleStackBehaviour>
{
    
}

public class CurrencySingleStackBehaviour : MonoBehaviour, IResetable, ICurrencyHolder
{
    [Inject] private CurrencySingleStackPool singleStackPool;
    [Inject] private CurrencyPool currencyPool;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform moneyParent;
    
    private CurrencyBehaviour currency;
    
    public void Initialize(int amount)
    {
        currency = currencyPool.GetElement();
        currency.transform.SetParent(moneyParent, false);
        currency.Init(amount);
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
        
    }

    public void OnPool()
    {

    }

    public void AddForce(Vector3 force)
    {
        rb.AddForce(force, ForceMode.VelocityChange);
    }
}
