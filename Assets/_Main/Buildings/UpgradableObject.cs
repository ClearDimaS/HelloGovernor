using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[Serializable]
public class UpgradableData
{
    public int spentMoney;
    public int level;
    public int optionIndex;
}

[RequireComponent(typeof(UpgradableSavable))]
public abstract class UpgradableObject : MonoBehaviour, IDataHolder<UpgradableData>
{
    [SerializeField] private bool autoGrantLevel1;
    [SerializeField] protected ScaleAnimator[] levels;
    [SerializeField] private MoneyConsumer moneyConsumer;

    private List<Price> levelPrices;
    protected UpgradableData data;
    
    public int LevelsCount => levels.Length;
    public int UpgradesCount => autoGrantLevel1 ? LevelsCount - 1 : LevelsCount;
    public bool IsBought => data.level > 0;
    
    private void Awake()
    {
        if (moneyConsumer == null)
        {
            moneyConsumer = GetComponentInChildren<MoneyConsumer>();
        }
        moneyConsumer.reachGoalEvent += LevelUp;
    }

    public void Initialize(UpgradableData data)
    {
        if (autoGrantLevel1 && data.level == 0)
        {
            data.level = 1;
        }

        levelPrices = GetPrices();
        this.data = data;
        RefreshLevelGFX(true);
        SetPrice();
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
            moneyConsumer.gameObject.SetActive(false);
        }
    }
}