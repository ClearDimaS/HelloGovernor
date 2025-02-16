using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

[Serializable]
public class BankAssistantData
{
    public int money;
}

public class BankAssistant : MonoBehaviour, IDataHolder<BankAssistantData>, ICurrencyHolder
{
    [Inject] protected CurrencyPool currencyPool;
    [Inject] protected PlayerDataRepository repository;
    [Inject] protected GameConfig config;
    
    [SerializeField] protected Walker walker;

    protected BankBuilding bank;
    protected BankAssistantData saveData;
    protected CurrencyStackBehaviour target;
    protected List<CurrencyStackBehaviour> stacks = new ();

    private void Awake()
    {
        stacks = FindObjectsOfType<CurrencyStackBehaviour>(true).ToList();
        bank = GetComponentInParent<BankBuilding>();
    }

    public BankAssistantData GetData()
    {
        return saveData;
    }

    public void Initialize(BankAssistantData data)
    {
        saveData = data;
    }
    
    private void Update()
    {
        if (saveData.money > 0)
        {
            walker.MoveToTarget(bank.AssistantPlace.position, AddMoneyToPlayer);  
        }
        else
        {
            if (target == null || target.GetMoney() <= 0)
            {
                RefreshTarget();
            }
            else
            {
                walker.MoveToTarget(target.transform.position, TakeMoney);  
            }   
        }
    }

    private void TakeMoney()
    {
        if (target != null)
        {
            target.Remove(this);   
        }
    }

    private void AddMoneyToPlayer()
    {
        repository.Money += saveData.money;
        saveData.money = 0;
    }

    private void RefreshTarget()
    {
        for (int i = 0; i < stacks.Count; i++)
        {
            if (stacks[i].IsFull())
            {
                target = stacks[i];
            }
        }
    }

    public void AddCurrency(int amount)
    {
        saveData.money += amount;
    }

    public void MoveCurrencyToMe(CurrencyBehaviour currency)
    {
        currency.transform.DOMove(transform.position, 0.3f);
        currency.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
        {
            saveData.money += currency.Amount;
            currencyPool.Pool(currency);
        });
    }
}
