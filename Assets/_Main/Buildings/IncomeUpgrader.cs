using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IncomeUpgrader : UpgradableObject
{
    [SerializeField] private float[] incomeMultipliers;
    [SerializeField] private string[] titles;
    private WishGranter wishGranter;

    private UpgradableBuilding building
    {
        get
        {
            if (_building == null)
            {
                _building = GetComponentInParent<UpgradableBuilding>();
            }

            return _building;
        }
    }
    protected UpgradableBuilding _building;

    protected override void OnAwake()
    {
        base.OnAwake();
        if (_building == null)
        {
            _building = GetComponentInParent<UpgradableBuilding>();
        }
        wishGranter = GetComponentInParent<WishGranter>();
        if (wishGranter == null || building == null)
        {
            Debug.LogError($"couldnt place income upgrader! my parent: {transform.parent}");
        }
        Consumer.transform.position = building.Consumer.transform.position + Vector3.forward * 1.15f;
        Consumer.transform.rotation = building.Consumer.transform.rotation;
    }

    protected override List<Price> GetPrices()
    {
        return building.GetPricesCopy().Select(x => new Price(x.price * 3, 0)).ToList();
    }

    public override Sprite GetPurchaseIcon()
    {
        return building.GetPurchaseIcon();
    }

    public override string GetTitle()
    {
        return titles[Mathf.Clamp(Level - 1, 0, titles.Length - 1)];;
    }

    public float GetMultiplier()
    {
        return incomeMultipliers[Mathf.Clamp(Level - 1, 0, titles.Length - 1)];
    }

    protected override void RefreshLevelGFX(bool instant)
    {
        base.RefreshLevelGFX(instant);
        if (IsBought)
        {
            wishGranter.IncomeMultiplier = GetMultiplier();
        }
    }
}
