using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class PlayerXPController : MonoBehaviour
{
    [Inject] private PlayerDataRepository dataRepository;
    [Inject] private GameConfig gameConfig;
    [Inject] private CacheManager cacheManager;
    [Inject] private UI_Manager uiManager;
    [Inject] private PlayerController playerController;
    
    [SerializeField] private float waitPause = 1f;

    private UI_RewardsPanel rewardsPanel;
    private float waitTime = 0f;

    public int Exp
    {
        get => cacheManager.XP;
        set => cacheManager.XP = value;
    }

    public int MaxExp
    {
        get => GetNextLevelData().maxXP;
    }

    public int LevelIndex
    {
        get => cacheManager.LevelIndex;
        set => cacheManager.LevelIndex = value;
    }

    private PlayerLevelData GetNextLevelData()
    {
        var levelIndex = Mathf.Clamp(LevelIndex, 0, gameConfig.levelUps.Length-1);
        return gameConfig.levelUps[levelIndex];
    }

    private void Start()
    {
        rewardsPanel = uiManager.GetPanel<UI_RewardsPanel>();
        GrantAllRewards();
    }

    private void GrantAllRewards()
    {
        for (var i = 0; i < gameConfig.levelUps.Length; i++)
        {
            if (LevelIndex > i)
            {
                var levelUpData = gameConfig.levelUps[i];
                GrantLevelUpData(levelUpData, i, false);
            }
        }
    }

    private void Update()
    {
        if (Exp >= MaxExp)
        {
            waitTime += Time.deltaTime;
            if (waitPause > waitTime)
            {
                waitTime = 0f;
                Exp -= MaxExp;
                LevelIndex++;
            }
        }
    }
    
    public void GrantLevelUpData(PlayerLevelData levelUpData, int levelIndex, bool isNew)
    {
        if (!dataRepository.HasGrantedMoney(levelIndex))
        {
            levelUpData.moneyReward.Grant(dataRepository);
            dataRepository.AddGrantedMoney(levelIndex);
        }
        levelUpData.capacityReward.Grant(playerController, isNew);
        levelUpData.speedReward.Grant(playerController, isNew);
    }

    public PlayerLevelData GetPrevLevelData()
    {
        var levelIndex = Mathf.Clamp(LevelIndex-1, 0, gameConfig.levelUps.Length-1);
        return gameConfig.levelUps[levelIndex];
    }

    [Button]
    public void AddXP(int value, Transform from)
    {
        Exp += value;
        rewardsPanel.SpawnXP(value, from.position, null);
    }
}