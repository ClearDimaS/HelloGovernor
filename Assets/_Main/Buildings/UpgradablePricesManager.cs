using System;
using System.Collections;
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
    public int thisTypeIndex;
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
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private int firstPrice;
    [SerializeField] private int secondPrice;
    [SerializeField] private List<MoneyConsumerData> upgradablePriceDatas;
    [SerializeField] private Vector2 priceGrowthFactorMinMax = new Vector2(1.05f, 1.4f);
    [SerializeField] private int[] maxPriceChanges = new int[]{3900, 6000, 4500};
    [SerializeField] private int[] minPriceChanges = new int[]{500, 1500, 1000};
    [SerializeField] private int seedForRequiredPurchases;
    private MoneyConsumerData waitingUnlock;
    private MoneyConsumerData lastUnlocked;
    
    private Queue<MoneyConsumerData> unlockQueue = new ();
    private Dictionary<UpgradableObject, List<Price>> consumersDict = new ();

    private List<UpgradableBuilding> available = new();
    private HashSet<WishGranter> bought = new();
    public int AvailableCount => available.Count;
    private bool allowNext = true;

    private void Awake()
    {
        //CollectAllConsumers();
    }

    private void Start()
    {
        SetAllLocked();
        foreach (var data in upgradablePriceDatas)
        {
            unlockQueue.Enqueue(data);
        }
        lastUnlocked = unlockQueue.Dequeue();
        if (lastUnlocked.upgradable is UpgradableBuilding building1 && !available.Contains(building1))
        {
            available.Add(building1);
        }

        bought = GetComponentsInChildren<WishGranter>().Where(x => x.GetComponent<UpgradableBuilding>() == null).ToHashSet();
        while (IsBought(lastUnlocked) && unlockQueue.Count > 0)
        {
            lastUnlocked = unlockQueue.Dequeue();
            if (lastUnlocked.upgradable is UpgradableBuilding building)
            {
                if (!available.Contains(building))
                {
                    available.Add(building);   
                }

                var granter = building.GetComponent<WishGranter>();
                if (granter != null)
                {
                    bought.Add(granter);
                }
            }
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
            if (IsBought(lastUnlocked) && (allowNext || lastUnlocked.level > 1))
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
                        if (lastUnlocked != null)
                        {
                            if (lastUnlocked.upgradable is UpgradableBuilding building1)
                            {
                                var granter = building1.GetComponent<WishGranter>();
                                if (granter != null)
                                {
                                    bought.Add(granter);
                                }  
                            }
                        }
                        lastUnlocked = unlockQueue.Dequeue();
                        if (lastUnlocked.level <= 1)
                        {
                            allowNext = false;
                        }
                        if (lastUnlocked.upgradable is UpgradableBuilding building && !available.Contains(building))
                        {
                            available.Add(building);
                        }
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
                    if (lastUnlocked.upgradable is UpgradableBuilding building && !available.Contains(building))
                    {
                        available.Add(building);
                    }
                    allowNext = false;
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

    public bool IsCurrentBought(UpgradableObject upgradableObject)
    {
        if (lastUnlocked == null || lastUnlocked.upgradable != upgradableObject)
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
        startCallback?.Invoke();
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
        return;
        UnityEngine.Random.InitState(seedForRequiredPurchases);
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
                var factorT = i / (float)upgradablePriceDatas.Count;
                var factor = Mathf.Lerp(priceGrowthFactorMinMax.y, priceGrowthFactorMinMax.x, factorT);
                var newPrice = Mathf.RoundToInt(upgradablePriceDatas[i - 1].price * factor) / 10 * 10;
                var minPriceChange = minPriceChanges[i % minPriceChanges.Length];
                var maxPriceChange = maxPriceChanges[i % maxPriceChanges.Length];
                if (newPrice - upgradablePriceDatas[i - 1].price > maxPriceChange)
                {
                    newPrice = upgradablePriceDatas[i - 1].price + maxPriceChange;
                }
                if (newPrice - upgradablePriceDatas[i - 1].price < minPriceChange)
                {
                    newPrice = upgradablePriceDatas[i - 1].price + minPriceChange;
                }

                if (upgradablePriceDatas[i].level > 1)
                {
                    var prevLevel = upgradablePriceDatas.FirstOrDefault(x =>
                        x.upgradable == upgradablePriceDatas[i].upgradable && x != upgradablePriceDatas[i]);
                    upgradablePriceDatas[i].price = Mathf.RoundToInt(gameConfig.upgradeIncomePriceMult * prevLevel.price)/10 * 10;
                }
                else
                {
                    upgradablePriceDatas[i].price = newPrice;
                }
            }
            
            var set = new HashSet<UpgradableObject>();
            for (int j = 0; j < i; j++)
            {
                if (upgradablePriceDatas[i].upgradable != upgradablePriceDatas[j].upgradable)
                {
                    set.Add(upgradablePriceDatas[j].upgradable);
                };
            }
            upgradablePriceDatas[i].thisTypeIndex = set.Count;
        }
    }

    public bool IsLast()
    {
        return unlockQueue.Count == 0 && IsBought(lastUnlocked);
    }

    public List<UpgradableBuilding> GetAvailableBuildings()
    {
        return available;
    }
    
    public HashSet<WishGranter> GetBoughtGranters()
    {
        return bought;
    }

    public void AllowNext()
    {
        allowNext = true;
    }

    public List<MoneyConsumerData> GetPurchaseSequence()
    {
        return upgradablePriceDatas;
    }

    public T GetBuildingsOfType<T>() where T : UpgradableBuilding
    {
        foreach (var building in available)
        {
            if (building.GetType() == typeof(T))
            {
                return building as T;
            }
        }

        foreach (var queue in unlockQueue)
        {
            if (queue.upgradable is T building)
            {
                return building;
            }
        }

        return null;
    }
}