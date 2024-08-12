using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(UpgradableSavable))]
public abstract class UpgradableObject : MonoBehaviour, IDataHolder<UpgradableData>
{
    [SerializeField] private bool autoGrantLevel1;
    [SerializeField] protected ScaleAnimator[] levels;
    [SerializeField] private MoneyConsumer moneyConsumer;

    private List<Price> levelPrices;
    protected UpgradableData data;
    protected int allowedLevelToPurchase;
    protected bool IsAllowedToBuy => Level < allowedLevelToPurchase;
    
    public int LevelsCount => levels.Length;
    public int UpgradesCount => autoGrantLevel1 ? LevelsCount - 1 : LevelsCount;
    public bool IsBought => data.level > 0;
    public int Level => data != null ? data.level : -1;
    public Transform BuyPlace => moneyConsumer.transform;
    public bool AutoGrantLevel1 => autoGrantLevel1;
    public int Price => moneyConsumer.Price;
    public int SpentAmount => data.spentMoney;

    private bool isPriceSet = false;
    private event Action upgradeEvent;

    private void Awake()
    {
        if (moneyConsumer == null)
        {
            moneyConsumer = GetComponentInChildren<MoneyConsumer>();
        }
        moneyConsumer.reachGoalEvent += LevelUp;
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        
    }

    private void Update()
    {
        if (!isPriceSet)
        {
            isPriceSet = true;
            SetPrice();
            RefreshLevelGFX(true);
        }
        data.spentMoney = moneyConsumer.GetSpentAmount();
    }

    public virtual void Initialize(UpgradableData data)
    {
        if (autoGrantLevel1 && data.level == 0)
        {
            data.level = 1;
        }

        levelPrices = GetPrices();
        this.data = data;
        isPriceSet = false;
    }

    public void SubscribeUpgrade(Action handler)
    {
        upgradeEvent += handler;
    }

    protected abstract List<Price> GetPrices();

    public UpgradableData GetData()
    {
        if (moneyConsumer == null)
        {
            Debug.LogWarning($"consumer is null: {moneyConsumer}");
            return null;
        }

        if (data == null)
        {
            Debug.LogWarning($"data is null: {moneyConsumer}");
            return null;
        }
        data.spentMoney = moneyConsumer.GetSpentAmount();
        return data;
    }
    
    private void LevelUp()
    {
        data.spentMoney = 0;
        data.level++;
        upgradeEvent?.Invoke();
        RefreshLevelGFX(false);
        SetPrice();
    }

    protected virtual void RefreshLevelGFX(bool instant)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (data.level == i + 1)
            {
                levels[i].Show(instant);
            }
            else
            {
                levels[i].Hide(instant);
            }
        }
    }

    private void SetPrice()
    {
        if (data.level >= 0 && data.level < levelPrices.Count)
        {
            var requiredPrice = levelPrices[data.level];
            var price = new Price(requiredPrice.price, data.spentMoney);
            moneyConsumer.SetPrice(price);   
        }
        else
        {
            moneyConsumer.SetPrice(new Price(1, 0));   
        }
        moneyConsumer.gameObject.SetActive(IsAllowedToBuy);
    }

    public void SetAllowBuy(int allowedLevel, bool instant)
    {
        allowedLevelToPurchase = allowedLevel;
        if (IsAllowedToBuy)
        {
            moneyConsumer.gameObject.SetActive(true);
            moneyConsumer.Show(instant);
        }
        else
        {
            moneyConsumer.gameObject.SetActive(false);
            moneyConsumer.Hide(instant);
        }
    }

    public abstract Sprite GetItemIcon();

    public abstract string GetTitle();
}