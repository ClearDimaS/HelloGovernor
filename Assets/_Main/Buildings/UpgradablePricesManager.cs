using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[Serializable]
public class MoneyConsumerData
{
    [ReadOnly] public int level;
    public int price;
    public UpgradableObject upgradable;

    public MoneyConsumerData(int price, UpgradableObject upgradable, int level)
    {
        this.price = price;
        this.upgradable = upgradable;
        this.level = level;
    }
}

public class UpgradablePricesManager : MonoBehaviour
{
    [Inject] private GameConfig gameConfig;
    [Inject] private CameraManager cameraManager;
    
    [SerializeField] private int firstPrice;
    [SerializeField] private int secondPrice;
    [SerializeField] private List<MoneyConsumerData> upgradablePriceDatas;

    private MoneyConsumerData lastUnlocked;
    
    private Queue<MoneyConsumerData> unlockQueue = new ();
    private Dictionary<UpgradableObject, List<Price>> consumersDict = new ();
    
    private void Awake()
    {
        CollectAllConsumers();
    }

    private void Start()
    {
        SetAllLocked();
        foreach (var data in upgradablePriceDatas)
        {
            unlockQueue.Enqueue(data);
        }
        lastUnlocked = unlockQueue.Dequeue();
        while (IsBought(lastUnlocked) && unlockQueue.Count > 0)
        {
            lastUnlocked = unlockQueue.Dequeue();
        }

        if (!IsBought(lastUnlocked))
        {
            AllowBuy(lastUnlocked);
        }
    }

    private void Update()
    {
        if (unlockQueue.Count > 0)
        {
            if (IsBought(lastUnlocked))
            {
                lastUnlocked = unlockQueue.Dequeue();
                AllowBuy(lastUnlocked, gameConfig.unlockCameraDelay);
            }
        }
    }

    public float GetProgress()
    {
        var addOne = IsBought(lastUnlocked) ? 0 : 1;
        return 1f - (unlockQueue.Count + addOne) / (float)upgradablePriceDatas.Count;
    }

    public List<Price> GetLevelPrices(UpgradableObject upgradable)
    {
        var prices = GetLevelPricesInternal(upgradable);
        return prices;
    }
    
    private bool IsBought(MoneyConsumerData data)
    {
        return data.upgradable.Level >= data.level;
    }
    
    private void AllowBuy(MoneyConsumerData moneyConsumerData, float delay = -1f)
    {
        Action startCallback = () => lastUnlocked.upgradable.SetAllowBuy(moneyConsumerData.level, false);
        var target = moneyConsumerData.upgradable.BuyPlace;
        var sp = cameraManager.ActiveCamera.WorldToViewportPoint(target.position);

        if (sp.x < gameConfig.cameraUnlockXBorders.x ||
            sp.y < gameConfig.cameraUnlockYBorders.x ||
            sp.x > gameConfig.cameraUnlockXBorders.y || 
            sp.y > gameConfig.cameraUnlockYBorders.y)
        {
            cameraManager.SetTarget(target, gameConfig.unlockCameraTimer, delay, startCallback);
        }
        else
        {
            startCallback?.Invoke();
        }
    }
    
    private void SetAllLocked()
    {
        foreach (var data in upgradablePriceDatas)
        {
            data.upgradable.SetAllowBuy(-1, true);
        }
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
            var levelAdd = onScene.AutoGrantLevel1 ? 1 : 0;
            var needUpgradesCount = onScene.UpgradesCount;
            var countCreated = upgradablePriceDatas.Count(x => System.Object.ReferenceEquals(x.upgradable, onScene));
            
            for (int i = countCreated; i < needUpgradesCount; i++)
            {
                var levelPurchase = i + levelAdd + 1;
                upgradablePriceDatas.Add(new MoneyConsumerData(0, onScene, levelPurchase));
            }
            

            for (int i = 0; i < countCreated - needUpgradesCount; i++)
            {
                var removeData = upgradablePriceDatas.First(x => x.upgradable == onScene);
                upgradablePriceDatas.Remove(removeData);
            }
        }

        Dictionary<UpgradableObject, int> levelsDict = new ();
        foreach (var data in upgradablePriceDatas)
        {
            if (!levelsDict.ContainsKey(data.upgradable))
            {
                levelsDict[data.upgradable] = data.upgradable.AutoGrantLevel1 ? 2 : 1;
            }
            else
            {
                levelsDict[data.upgradable]++;
            }
            data.level = levelsDict[data.upgradable];
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