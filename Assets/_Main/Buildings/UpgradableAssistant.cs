using System.Collections.Generic;
using UnityEngine;

public class UpgradableAssistant : UpgradableObject
{
    [SerializeField] private Transform boughtRoot;

    private UpgradableBuilding upgradableBuilding;
    protected List<Price> pricesCopy;
    
    private int spawnedLevel = -2;

    protected override List<Price> GetPrices()
    {
        if (upgradableBuilding == null)
        {
            upgradableBuilding = GetComponentInParent<UpgradableBuilding>(true);
        }
        if (pricesCopy == null)
        {
            pricesCopy = upgradableBuilding.GetPricesCopy();
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
}