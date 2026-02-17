using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UpgradableOperator : UpgradableObject
{
    [Inject] private GameConfig gameConfig;
    [Inject] protected TutorialsConfig tutorialsConfig;
    
    [SerializeField] private Transform boughtRoot;

    private OperatableGranter itemsWishGranter;
    private UpgradableBuilding upgradableBuilding;
    protected List<Price> pricesCopy;
    
    private int spawnedLevel = -2;

    public override void Initialize(UpgradableData data)
    {
        this.allowedLevelToPurchase = LevelsCount;
        base.Initialize(data);
    }

    protected override List<Price> GetPrices()
    {
        if (upgradableBuilding == null)
        {
            upgradableBuilding = GetComponentInParent<UpgradableBuilding>(true);
        }
        if (pricesCopy == null)
        {
            pricesCopy = upgradableBuilding.GetPricesCopy();
            foreach (var price in pricesCopy)
            {
                price.price = Mathf.RoundToInt(gameConfig.assistantPricesMult * price.price) / 10 * 10;
            }
        }
        
        return pricesCopy;
    }

    protected override void RefreshLevelGFX(bool instant)
    {
        base.RefreshLevelGFX(instant);
        if (spawnedLevel != data.level && data.level > 0)
        {
            if (!boughtRoot.gameObject.activeSelf)
            {
                boughtRoot.gameObject.SetActive(true);
            }
            
            spawnedLevel = data.level;
        }
        else
        {
            if (boughtRoot.gameObject.activeSelf != IsBought)
            {
                boughtRoot.gameObject.SetActive(IsBought);   
            }
        }
    }

    public override Sprite GetPurchaseIcon()
    {
        if (itemsWishGranter == null)
        {
            itemsWishGranter = GetComponentInParent<OperatableGranter>(true);
        }
        return itemsWishGranter.GetIconOperator();
    }
    
    public override string GetTitle()
    {
        return string.Format(tutorialsConfig.Cashier, upgradableBuilding.GetTitle());
    }
}