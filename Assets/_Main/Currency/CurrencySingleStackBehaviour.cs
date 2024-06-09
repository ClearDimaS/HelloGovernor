using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CurrencySingleStackBehaviour : MonoBehaviour, IResetable, ICurrencyHolder
{
    [Inject] private CurrencySingleStackPool singleStackPool;
    [Inject] private CurrencyPool currencyPool;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform moneyParent;

    private float initTime;
    private float pickUpDelay = 1f;
    private CurrencyBehaviour currency;
    
    public void Initialize(int amount)
    {
        currency = currencyPool.GetElement();
        currency.transform.SetParent(moneyParent, false);
        currency.transform.localPosition = Vector3.zero;
        currency.transform.localRotation = Quaternion.identity;
        currency.Init(amount);
        initTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null || other.isTrigger)
        {
            return;
        }

        if (other.attachedRigidbody.TryGetComponent(out PlayerController player))
        {
            if (Time.time - initTime > pickUpDelay)
            {
                Remove(player);   
            }
            else
            {
                UniTask.Delay(TimeSpan.FromSeconds(pickUpDelay - (Time.time - initTime))).ContinueWith(() =>
                {
                    rb.isKinematic = true;
                });
            }
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
        
    }

    public void OnPool()
    {

    }

    public void AddForce(Vector3 force)
    {
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.VelocityChange);
    }
}
