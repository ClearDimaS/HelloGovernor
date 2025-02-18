using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/WishesConfig", fileName = "WishConfig")]
public class WishesCollectionConfig : TypedCollectionConfig<WishGranterConfig, Type>
{
    public Vector2Int chatGroupSizeMinMax;
    [Header("Break house")]
    public Vector2 breakTimerMinMax;
    public float repairHouseTime;
    public int repairHouseReward = 10;

    [Header("Houses")] 
    public int[] CitizenCountsForHouseLevels;
    [Header("Bank")]
    public Vector2Int bankRewardMinMaxPositive;
    public Vector2Int bankRewardMinMaxNegative;
    [Range(0, 1f)] public float bankNegativeProbability;
    
    public float GetGrantDuration(WishGranter granter)
    {
        return GetItem(granter.GetType()).grantDuration;
    }
    
    public int GetReward(WishGranter granter)
    {
        return GetItem(granter.GetType()).reward;
    }

    public Type GetRandomWishType()
    {
        return GetRandomElementByWeight(collection).Key;
    }
    
    private WishGranterConfig GetRandomElementByWeight(List<WishGranterConfig> list)
    {
        float totalWeight = 0f;
        foreach (var item in list)
        {
            totalWeight += item.chanceWeight;
        }

        float randomNumber = UnityEngine.Random.Range(0f, totalWeight);

        float sum = 0f;
        foreach (var item in list)
        {
            sum += item.chanceWeight;
            if (randomNumber <= sum)
            {
                return item;
            }
        }
        throw new InvalidOperationException("Failed to select an element based on weight.");
    }

    public WishGranterConfig GetConfig(WishGranter granter)
    {
        return GetItem(granter.GetType());
    }

    public float GetCooldown(WishGranter granter)
    {
        return GetItem(granter.GetType()).coolDown;
    }
}