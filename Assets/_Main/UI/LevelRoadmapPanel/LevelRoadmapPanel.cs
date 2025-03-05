using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Button = UnityEngine.UI.Button;

public class LevelRoadmapPanel : UI_Panel
{
    [Inject] private CacheManager cacheManager;
    [Inject] private UI_Manager uiManager;
    [Inject] private DiContainer container;
    [Inject] private UpgradablePricesManager pricesManager;
    [Inject] private GameConfig gameConfig;

    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private List<RoadmapLevelInfo> roadmapLevelInfos;
    [SerializeField] private RoadmapLevelInfo levelPrefab;
    [SerializeField] private Transform levelsParent;

    private LevelUpPanel levelUpPanel;
    private HashSet<Type> upgradablesSeen = new ();

    private bool lastIsNew;
    protected override void OnAwake()
    {
        base.OnAwake();
        roadmapLevelInfos = levelsParent.GetComponentsInChildren<RoadmapLevelInfo>().ToList();
        acceptButton.onClick.AddListener(() =>
        {
            var levelData = GetClampedLevelData(cacheManager.LevelIndex);
            levelUpPanel.Show(cacheManager.LevelIndex, levelData);
            Hide();
        });
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        levelUpPanel = uiManager.GetPanel<LevelUpPanel>();
    }

    public void Show(bool isNew)
    {
        lastIsNew = isNew;
        closeButton.gameObject.SetActive(!isNew);
        acceptButton.gameObject.SetActive(isNew);
        
        Show();
    }

    private PlayerLevelData GetClampedLevelData(int levelIndex)
    {
        var clampedLevel = Mathf.Clamp(levelIndex, 0, gameConfig.levelUps.Length - 1);
        return gameConfig.levelUps[clampedLevel];
    }

    public override void OnShow()
    {
        base.OnShow();
        upgradablesSeen.Clear();
        var upgradesSequence = pricesManager.GetPurchaseSequence();
        var upgradesCounter = 0;
        var levelsToShow = Mathf.Max(gameConfig.levelUps.Length, cacheManager.LevelIndex + 1);
        for (var i = 0; i < levelsToShow; i++)
        {
            var uniqueUpgradesAtLevel = new List<UpgradableObject>();
            if (i >= roadmapLevelInfos.Count)
            {
                var newLevelElement =
                    container.InstantiatePrefabForComponent<RoadmapLevelInfo>(levelPrefab, levelsParent);
                roadmapLevelInfos.Add(newLevelElement);
            }

            var levelData = GetClampedLevelData(i);
            for (int j = 0; j < levelData.maxXP; j+=5)
            {
                if (upgradesCounter >= upgradesSequence.Count)
                {
                    break;
                }
                var upgradeAtThisLevel = upgradesSequence[upgradesCounter];
                var upgradable = upgradeAtThisLevel.upgradable;
                if (upgradable is UpgradableBuilding building)
                {
                    var upgradableType = building.Building.GetType();
                    if (!upgradablesSeen.Contains(upgradableType))
                    {
                        upgradablesSeen.Add(upgradableType);
                        uniqueUpgradesAtLevel.Add(upgradable);
                    }
                    upgradesCounter++;   
                }
            }
            
            var levelInfoElement = roadmapLevelInfos[i];
            levelInfoElement.Init(levelData, i, uniqueUpgradesAtLevel);
            levelInfoElement.Rect.SetAsFirstSibling();
        }

        if (scrollView.content.rect.height < 1)
        {
            UniTask.DelayFrame(1).ContinueWith(() =>
            {
                MoveToTarget();
            }).AddTo(gameObject);
        }
        else
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        var index = lastIsNew ? Mathf.Max(0, cacheManager.LevelIndex-1) : cacheManager.LevelIndex;
        var t = 0f;
        var endScroll = 1f - (Mathf.Abs(roadmapLevelInfos[index].Rect.anchoredPosition.y)/scrollView.content.rect.height);
        DOTween.To(() => t, x => t = x, endScroll, 0.3f).OnUpdate(() =>
        {
            scrollView.verticalNormalizedPosition = t;
        }).OnComplete(() =>
        {
            scrollView.verticalNormalizedPosition = endScroll;
        }).SetEase(Ease.InOutCirc);
    }
}
