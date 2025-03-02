using System;
using UnityEngine;

public class BuildingTutorialStep : TutorialStep
{
    protected UpgradableBuilding building;
    protected int levelIndex;
    protected int price;
    protected int typeIndex;
    public int LevelIndex => levelIndex;
    public override bool UseCache => false;

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

        return 0f;
    }

    public override Transform GetCameraTarget()
    {
        return building.BuyPlace;
    }

    public override Transform GetArrowTarget()
    {
        return building.BuyPlace;
    }

    private static string notBoughtProgressText = "0/1";
    private static string boughtProgressText = "1/1";
    
    protected override string CreateProgressText()
    {
        if (building.Level > levelIndex)
        {
            return boughtProgressText;
        }

        return notBoughtProgressText;
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

    public override Sprite GetTutorialIcon()
    {
        return building.GetPurchaseIcon();
    }

    public void ForcePurchase()
    {
        building.ForcePurchase();
    }
}