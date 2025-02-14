using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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

    private MoneyConsumerData waitingUnlock;
    private MoneyConsumerData lastUnlocked;
    
    private Queue<MoneyConsumerData> unlockQueue = new ();
    private Dictionary<UpgradableObject, List<Price>> consumersDict = new ();

    private List<UpgradableBuilding> bought = new();
    
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

    private int levelCounter = 0;
    private void Update()
    {
        if (unlockQueue.Count > 0)
        {
            if (IsBought(lastUnlocked))
            {
                if (waitingUnlock == unlockQueue.Peek())
                {
                    return;
                }
                
                if (waitingUnlock != null)
                {
                    waitingUnlock = unlockQueue.Peek();
                    UniTask.Delay(TimeSpan.FromSeconds(1f)).ContinueWith(() =>
                    {
                        lastUnlocked = unlockQueue.Dequeue();
                        AllowBuy(lastUnlocked, gameConfig.unlockCameraDelay);
                    });
                    
                    VodooAnalyticsManagerFacade.WinLevel(levelCounter);
                    //AnalyticsManager.Instance.WinLevel(levelCounter);
                    levelCounter++;
                    VodooAnalyticsManagerFacade.StartLevel(levelCounter);
                    //AnalyticsManager.Instance.StartLevel(levelCounter);
                }
                else
                {
                    waitingUnlock = unlockQueue.Peek();
                    lastUnlocked = unlockQueue.Dequeue();
                    if (lastUnlocked.upgradable is UpgradableBuilding building && !bought.Contains(building))
                    {
                        bought.Add(building);
                    }
                    AllowBuy(lastUnlocked, gameConfig.unlockCameraDelay);
                    
                    levelCounter = 1;
                    
                    VodooAnalyticsManagerFacade.StartLevel(levelCounter);
                    //AnalyticsManager.Instance.StartLevel(levelCounter);
                }
            }
        }
    }

    public UpgradableObject GetNextData()
    {
        return lastUnlocked.upgradable;
    }
    
    public float GetProgress()
    {
        if (lastUnlocked == null)
        {
            return 1f;
        }
        var addOne = IsBought(lastUnlocked) ? 0 : 1;
        return 1f - (unlockQueue.Count + addOne) / (float)upgradablePriceDatas.Count;
    }

    public List<Price> GetLevelPrices(UpgradableObject upgradable)
    {
        var prices = GetLevelPricesInternal(upgradable);
        return prices;
    }

    public bool IsCurrentBought()
    {
        if (lastUnlocked == null)
        {
            return true;
        }
        return IsBought(lastUnlocked);
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

        if ((sp.x < gameConfig.cameraUnlockXBorders.x ||
             sp.y < gameConfig.cameraUnlockYBorders.x ||
             sp.x > gameConfig.cameraUnlockXBorders.y || 
             sp.y > gameConfig.cameraUnlockYBorders.y) && gameConfig.showCameraOnUnlock)
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
            var upgradesCount = upgradableObject.UpgradesCount + (upgradableObject.AutoGrantLevel1 ? 1 : 0);
            var prices = new List<Price>();
            if (upgradableObject.AutoGrantLevel1)
            {
                prices.Add(new Price(10, 0));
            }
            for (int i = 0; i < upgradablePriceDatas.Count; i++)
            {
                if (upgradablePriceDatas[i].upgradable == upgradableObject)
                {
                    prices.Add(new Price(upgradablePriceDatas[i].price, 0));
                }
            }

            if (prices.Count != upgradesCount)
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

    [Button]
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

    public bool IsLast()
    {
        return unlockQueue.Count == 0 && IsBought(lastUnlocked);
    }

    public List<UpgradableBuilding> GetBoughtBuildings()
    {
        return bought;
    }
}