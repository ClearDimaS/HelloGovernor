using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/WishesConfig", fileName = "WishConfig")]
public class WishesConfig : TypedCollectionConfig<WishesConfig.WishData, Type>
{
    [Serializable]
    public class WishData : IKey<Type>
    {
        public WishGranter granter;
        private Type type;
        public float grantDuration;
        public int reward;
        public float chanceWeight;

        public Type Key
        {
            get
            {
                if (type == null)
                {
                    type = granter.GetType();
                }

                return type;
            }
        }
    }

    [SerializeField] private List<WishesConfig.WishData> moneyWishes;
    public Vector2Int chatGroupSizeMinMax;
    
    public float GetGrantDuration(WishGranter type)
    {
        return GetItem(type.GetType()).grantDuration;
    }
    
    public int GetReward(WishGranter type)
    {
        return GetItem(type.GetType()).reward;
    }

    public Type GetRandomWishType()
    {
        return GetRandomElementByWeight(collection).Key;
    }
    
    public Type GetRandomMoneyWishType()
    {
        return GetRandomElementByWeight(moneyWishes).Key;
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