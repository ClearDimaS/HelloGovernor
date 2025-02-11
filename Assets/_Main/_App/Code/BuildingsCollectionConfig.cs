using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Buildings/Collection", fileName = "BuildingsConfig")]
public class BuildingsCollectionConfig : TypedCollectionConfig<BuildingConfig, Type>
{
    public BuildingConfig GetBuildingData(UpgradableBuilding building)
    {
        var item = GetItem(building.GetType());
        return item;
    }
}