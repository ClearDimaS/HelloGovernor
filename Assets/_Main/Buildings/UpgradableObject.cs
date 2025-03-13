using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(UpgradableSavable))]
public abstract class UpgradableObject : CulledBehaviour, IDataHolder<UpgradableData>
{
    [Inject] private PlayerController playerController;
    
    [SerializeField] private bool autoGrantLevel1;
    [SerializeField] protected ScaleAnimator[] levels;
    [SerializeField] private MoneyConsumer moneyConsumer;

    public MoneyConsumer Consumer => moneyConsumer;
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

    protected override void OnAwake()
    {
        base.OnAwake();
        if (moneyConsumer == null)
        {
            moneyConsumer = GetComponentInChildren<MoneyConsumer>();
        }
        moneyConsumer.reachGoalEvent += LevelUp;
    }

    public int GetPriceForLevel(int level)
    {
        return levelPrices[level].price;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
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
    
    protected void LevelUp()
    {
        data.spentMoney = 0;
        data.level++;
        upgradeEvent?.Invoke();
        RefreshLevelGFX(false);
        OnUpgrade();
        SetPrice();
        playerController.AddBoughtBuilding(this);
    }

    protected virtual void OnUpgrade()
    {
        
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
        if (allowedLevel >= 1)
        {
            allowedLevel = levels.Length;
        }
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

    public abstract Sprite GetPurchaseIcon();

    public abstract string GetTitle();

    public void ForcePurchase()
    {
        LevelUp();
    }
}