using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class PlayerXPController : MonoBehaviour
{
    [Inject] private GameConfig gameConfig;
    [Inject] private CacheManager cacheManager;
    
    [SerializeField] private float waitPause = 1f;
    
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

    private void Update()
    {
        if (Exp >= MaxExp)
        {
            waitTime += Time.deltaTime;
            if (waitPause > waitTime)
            {
                waitTime = 0f;
                LevelIndex++;
                Exp = 0;
            }
        }
    }

    public PlayerLevelData GetPrevLevelData()
    {
        var levelIndex = Mathf.Clamp(LevelIndex-1, 0, gameConfig.levelUps.Length-1);
        return gameConfig.levelUps[levelIndex];
    }

    [Button]
    public void AddXP(int value)
    {
        Exp += value;
    }
}