using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Buildings/Collection", fileName = "BuildingsConfig")]
public class BuildingsCollectionConfig : TypedCollectionConfig<BuildingConfig, Type>
{
    [field: SerializeField, PropertyOrder(-1)] public Sprite HelperIcon { get; protected set; }
    
    public BuildingConfig GetBuildingData(BuildingBase building)
    {
        var item = GetItem(building.GetType());
        return item;
    }
}