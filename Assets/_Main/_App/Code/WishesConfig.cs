using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/WishesConfig", fileName = "WishConfig")]
public class WishesConfig : TypedCollectionConfig<WishesConfig.WishData, EWish>
{
    [Serializable]
    public class WishData : IKey<EWish>
    {
        public EWish type;
        public float grantDuration;
        public int reward;
        public float chanceWeight;
        
        public EWish Key => type;
    }
    
    public Vector2Int chatGroupSizeMinMax;
    
    public float GetGrantDuration(EWish type)
    {
        return GetItem(type).grantDuration;
    }
    
    public int GetReward(EWish type)
    {
        return GetItem(type).reward;
    }

    public EWish GetRandomWishType()
    {
        return GetRandomElementByWeight(collection).type;
    }
    
    private WishData GetRandomElementByWeight(List<WishData> list)
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
}