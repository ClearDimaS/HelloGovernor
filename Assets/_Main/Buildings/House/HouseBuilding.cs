using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HouseBuilding : BuildingBase
{
    [Inject] private HousesManager housesManager;

    [SerializeField] private List<HouseRepairable> repairables;
    [field: SerializeField] public Transform CitizenPlace { get; private set; }

    public int CitizensCount => 16;

    protected override void OnAwake()
    {
        base.OnAwake();
        housesManager.Add(this);
    }

    public bool CanRepair(IRepairer repairer, out IRepairable target)
    {
        foreach (var repairable in repairables)
        {
            if (repairable.CanRepair(repairer))
            {
                target = repairable;
                return true;
            }
        }

        target = null;
        return false;
    }
    
    public bool CanRepair(IRepairer repairer)
    {
        return CanRepair(repairer, out IRepairable target);
    }

    public void Break()
    {
        foreach (var repairable in repairables)
        {
            repairable.Break();
        }
    }

    public bool IsBroken()
    {
        foreach (var repairable in repairables)
        {
            if (repairable.IsBroken)
            {
                return true;
            }
        }

        return false;
    }
}
