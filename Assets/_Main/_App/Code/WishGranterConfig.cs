using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Wishes/WishGranter", fileName = "WishConfig")]
public abstract class WishGranterConfig : ScriptableObject, IKey<Type>
{
    public WishGranter granter;
    public float grantDuration;
    public int reward;
    public float chanceWeight;

    public BuildingData buildingData;
    private Type type;
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