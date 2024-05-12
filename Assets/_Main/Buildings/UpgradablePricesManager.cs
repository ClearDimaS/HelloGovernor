using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class MoneyConsumerData
{
    public int price;
    public UpgradableObject upgradable;

    public MoneyConsumerData(int price, UpgradableObject upgradable)
    {
        this.price = price;
        this.upgradable = upgradable;
    }
}

public class UpgradablePricesManager : MonoBehaviour
{
    [SerializeField] private int firstPrice;
    [SerializeField] private int secondPrice;
    [SerializeField] private List<MoneyConsumerData> upgradablePriceDatas;

    private Dictionary<UpgradableObject, List<Price>> consumersDict = new ();

    private void Awake()
    {
        CollectAllConsumers();
    }

    public List<Price> GetLevelPrices(UpgradableObject upgradable)
    {
        var prices = GetLevelPricesInternal(upgradable);
        return prices;
    }

    private List<Price> GetLevelPricesInternal(UpgradableObject upgradableObject)
    {
        if (!consumersDict.ContainsKey(upgradableObject))
        {
            var prices = new List<Price>();
            for (int i = 0; i < upgradablePriceDatas.Count; i++)
            {
                if (upgradablePriceDatas[i].upgradable == upgradableObject)
                {
                    prices.Add(new Price(upgradablePriceDatas[i].price, 0));
                }
            }

            if (prices.Count != upgradableObject.UpgradesCount)
            {
                throw new NotImplementedException($"prices for: {upgradableObject.transform.name} is not set correctly!");
            }
            consumersDict[upgradableObject] = prices;
        }

        return consumersDict[upgradableObject];
    }

    [Button]
    private void CollectAllConsumers()
    {
        for (int i = 0; i < upgradablePriceDatas.Count; i++)
        {
            if (upgradablePriceDatas[i].upgradable == null)
            {
                upgradablePriceDatas.RemoveAt(i);
                i--;
            }
        }
        
        var buildings = FindObjectsOfType<UpgradableBuilding>(true);
        var onSceneUpgradables = buildings;
        
        foreach (var onScene in onSceneUpgradables)
        {
            var needUpgradesCount = onScene.UpgradesCount;
            var countCreated = upgradablePriceDatas.Count(x => x.upgradable == onScene);
            for (int i = countCreated; i < needUpgradesCount; i++)
            {
                upgradablePriceDatas.Add(new MoneyConsumerData(0, onScene));
            }

            for (int i = 0; i < countCreated - needUpgradesCount; i++)
            {
                var removeData = upgradablePriceDatas.First(x => x.upgradable == onScene);
                upgradablePriceDatas.Remove(removeData);
            }
        }

        UpdatePrices();
    }

    private void UpdatePrices()
    {
        for (int i = 0; i < upgradablePriceDatas.Count; i++)
        {
            if (i == 0)
            {
                upgradablePriceDatas[i].price = firstPrice;
            }
            else if (i == 1)
            {
                upgradablePriceDatas[i].price = secondPrice;
            }
            else
            {
                upgradablePriceDatas[i].price = upgradablePriceDatas[i - 1].price * 2 - upgradablePriceDatas[i - 2].price;
            }
        }
    }
}