using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RoadmapLevelInfo : MonoBehaviour
{
    [Inject] protected CacheManager cacheManager;
    [Inject] protected PlayerController controller;
    
    [Header("XP")]
    [SerializeField] protected TMP_Text levelText;
    [SerializeField] protected Image xpFill;
    [SerializeField] protected GameObject hideIfMoreThan1Level;
    [SerializeField] protected GameObject[] doneGOs;
    [SerializeField] protected GameObject[] notDoneGOs;
    [SerializeField] protected GameObject[] notReachedContents;
    [SerializeField] protected GameObject[] reachedContents;

    [Header("Rewards")]
    [SerializeField] protected GameObject rewardsHeader;
    [SerializeField] protected RoadmapRewardElement rewardMoney;
    [SerializeField] protected RoadmapRewardElement rewardSpeed;
    [SerializeField] protected RoadmapRewardElement rewardCapacity;
    
    [Header("Unlocks")]
    [SerializeField] protected GameObject unlocksHeader;
    [SerializeField] protected BuildingUnlockElement buildingUnlockElementPrefab;
    [SerializeField] protected Transform buildingUnlocksParent;
    [SerializeField] protected List<BuildingUnlockElement> buildingUnlocks = new ();
    
    private PlayerLevelData levelData;
    private int levelIndex;
    protected List<UpgradableObject> newUpgradables;
    public RectTransform Rect { get; protected set; }

    private void Awake()
    {
        buildingUnlocks = buildingUnlocksParent.GetComponentsInChildren<BuildingUnlockElement>(true).ToList();
        Rect = GetComponent<RectTransform>();
    }

    public void Init(PlayerLevelData data, int levelIndex, List<UpgradableObject> newUpgradables)
    {
        this.levelData = data;
        this.levelIndex = levelIndex;
        this.newUpgradables = newUpgradables;

        RefreshLevel();
        RefreshReached();
        RefreshRewards();
        RefreshUnlocks();
    }

    private void RefreshLevel()
    {
        levelText.text = $"{levelIndex + 2}";
        var progress = GetXP_Progress();
        xpFill.rectTransform.FillParent(progress, horizontal:false);
    }

    private void RefreshReached()
    {
        var reached = controller.XP_Controller.LevelIndex >= levelIndex;
        var done = controller.XP_Controller.LevelIndex >= levelIndex + 1;
        var notNext = controller.XP_Controller.LevelIndex < levelIndex - 1 && !reached;
        if (hideIfMoreThan1Level.activeSelf != notNext)
        {
            hideIfMoreThan1Level.gameObject.SetActive(notNext);   
        }

        foreach (var go in doneGOs)
        {
            if (go.activeSelf != done)
            {
                go.SetActive(done);
            }
        }
        foreach (var go in notDoneGOs)
        {
            if (go.activeSelf == done)
            {
                go.SetActive(!done);
            }
        }
        foreach (var content in notReachedContents)
        {
            if (content.activeSelf == reached)
            {
                content.SetActive(!reached);
            }
        }
        foreach (var content in reachedContents)
        {
            if (content.activeSelf != reached)
            {
                content.SetActive(reached);
            }
        }
    }

    private void RefreshRewards()
    {
        rewardsHeader.gameObject.SetActive(levelData.moneyReward.amount > 0 || 
                                           levelData.speedReward.addPercents > 0 || 
                                           levelData.capacityReward.add > 0);
        var received = cacheManager.GrantedMoneys.Contains(levelIndex - 1);
        rewardMoney.gameObject.SetActive(levelData.moneyReward.amount > 0);
        rewardMoney.Refresh(levelData.moneyReward, received);
        
        rewardSpeed.gameObject.SetActive(levelData.speedReward.addPercents > 0);
        rewardSpeed.Refresh(levelData.speedReward, received);
        
        rewardCapacity.gameObject.SetActive(levelData.capacityReward.add > 0);
        rewardCapacity.Refresh(levelData.capacityReward, received);
    }

    private void RefreshUnlocks()
    {
        foreach (var unlock in buildingUnlocks)
        {
            unlock.gameObject.SetActive(false);
        }
        unlocksHeader.SetActive(newUpgradables.Count > 0);
        for (var i = 0; i < newUpgradables.Count; i++)
        {
            var upgradable = newUpgradables[i];
            if (i >= buildingUnlocks.Count)
            {
                var newUnlockElement = Instantiate(buildingUnlockElementPrefab, buildingUnlocksParent);
                buildingUnlocks.Add(newUnlockElement);
            }

            var unlockElement = buildingUnlocks[i];
            var isBought = upgradable.IsBought;
            unlockElement.gameObject.SetActive(true);
            unlockElement.Init(upgradable.GetPurchaseIcon(), isBought);
        }
    }
    
    private float GetXP_Progress()
    {
        if (controller.XP_Controller.LevelIndex == levelIndex)
        {
            return controller.XP_Controller.Exp / (float)controller.XP_Controller.MaxExp;
        }
        else if (controller.XP_Controller.LevelIndex > levelIndex)
        {
            return 1f;
        }
        else
        {
            return 0f;
        }
    }
}
