using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[Serializable]
public class UpgradableData
{
    public int spentMoney;
    public int level;
}

public class UpgradableObject : MonoBehaviour, IDataHolder<UpgradableData>
{
    [Inject] private UpgradablePricesManager _upgradablePricesManager;

    [SerializeField] private bool autoGrantLevel1;
    [SerializeField] private ScaleAnimator[] levels;
    [SerializeField] private MoneyConsumer moneyConsumer;

    private List<Price> levelPrices;
    private UpgradableData data;
    
    public int LevelsCount => levels.Length;
    public int UpgradesCount => autoGrantLevel1 ? LevelsCount - 1 : LevelsCount;
    
    private void Awake()
    {
        moneyConsumer.reachGoalEvent += LevelUp;
    }

    public void Initialize(UpgradableData data)
    {
        if (autoGrantLevel1 && data.level == 0)
        {
            data.level = 1;
        }
        levelPrices = _upgradablePricesManager.GetLevelPrices(this);
        this.data = data;
        RefreshLevelGFX(true);
        SetPrice();
    }

    public UpgradableData GetData()
    {
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

    private void RefreshLevelGFX(bool instant)
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