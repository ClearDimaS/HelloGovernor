using System;
using UnityEngine;

public class BuildingTutorialStep : TutorialStep
{
    protected UpgradableBuilding building;
    protected int levelIndex;
    protected int price;
    protected int typeIndex;
    
    public BuildingTutorialStep(UpgradableBuilding building, int levelIndex, int typeIndex, PlayerDataRepository repository) : base(repository)
    {
        this.building = building;
        this.levelIndex = levelIndex;
        this.price = building.GetPriceForLevel(levelIndex);
        this.typeIndex = typeIndex;
    }

    protected override string CreateKey()
    {
        return $"buy_{building.GetType().Name}({typeIndex})_level_{levelIndex}";
    }

    protected override void UpdateProgress_Internal()
    {
        
    }

    public override float GetProgress()
    {
        if (building.Level > levelIndex)
        {
            return 1f;
        }

        return building.SpentAmount / (float)price;
    }

    public override Transform GetCameraTarget()
    {
        return building.transform;
    }

    public override Transform GetArrowTarget()
    {
        return building.BuyPlace;
    }

    protected override string CreateProgressText()
    {
        if (building.Level > levelIndex)
        {
            return $"{price}/{price}";
        }

        return $"{building.SpentAmount}/{price}";
    }

    protected override string CreateTitle()
    {
        if (levelIndex == 0)
        {
            return $"Buy {building.GetTitle()}";   
        }
        else
        {
            return $"Upgrade {building.GetTitle()}";
        }
    }
}