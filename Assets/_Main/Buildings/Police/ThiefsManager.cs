using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class ThiefsManager : MonoBehaviour
{
    [Inject] protected DayTimeManager dayTimeManager;
    [Inject] protected CameraManager cameraManager;
    [Inject] protected ThiefsPool thiefsPool;
    [Inject] protected GameConfig config;

    [SerializeField] protected ThiefSpawnData[] thiefSpawns;
    protected float lastStealTime;
    protected bool isStealing;

    protected ThiefSpawnData lastSpawnData;
    
    public bool IsActiveStealing => IsStealing && activeThief != null && !activeThief.IsBusted;
    public bool IsStealing => isStealing;
    protected ThiefBehaviour activeThief;

    
    protected List<CurrencyStackBehaviour> stacks = new ();
    
    private void Awake()
    {
        stacks = FindObjectsOfType<CurrencyStackBehaviour>(true).ToList();
    }

    private void Update()
    {
        if (isStealing)
        {
            return;
        }
        
        if (Time.time - lastStealTime > config.thiefPause)
        {
            var target = GetThiefTarget(false);
            if (target != null && dayTimeManager.IsLampsEnabled)
            {
                SpawnThief(target);
            }
        }
    }

    public CurrencyStackBehaviour GetThiefTarget(bool any = true)
    {
        for (int i = 0; i < stacks.Count; i++)
        {
            if (stacks[i].GetMoney() > config.moneyToSteal)
            {
                return stacks[i];
            }
        }

        if (any)
        {
            for (int i = 0; i < stacks.Count; i++)
            {
                if (stacks[i].GetMoney() > 0)
                {
                    return stacks[i];
                }
            }   
        }

        return null;
    }

    private void SpawnThief(CurrencyStackBehaviour target)
    {
        activeThief = thiefsPool.GetElement();
        var spawnData = thiefSpawns[Random.Range(0, thiefSpawns.Length)];
        activeThief.transform.position = spawnData.spawn.position;
        activeThief.Init(target, spawnData.escape);
        isStealing = true;

        cameraManager.SetTarget(activeThief.transform, 2f);
        lastSpawnData = spawnData;
    }

    public void FinishSteal(ThiefBehaviour thief)
    {
        lastStealTime = Time.time;
        isStealing = false;
        thiefsPool.Pool(thief);
        activeThief = null;
    }

    public ThiefBehaviour GetStealer()
    {
        return activeThief;
    }

    public float GetRobberyNormalizedProgress()
    {
        if (!isStealing)
        {
            return 0f;
        }
        
        return (activeThief.transform.position - lastSpawnData.spawn.position).magnitude / (lastSpawnData.spawn.position - lastSpawnData.escape.position).magnitude;
    }

    public bool HasBusted()
    {
        return isStealing && activeThief != null && activeThief.IsBusted;
    }
}