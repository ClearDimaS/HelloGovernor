using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HouseBuilding : BuildingBase
{
    [Inject] private WishesCollectionConfig gameConfig;
    [Inject] private HousesManager housesManager;

    [SerializeField] private List<BoxCollider> houseTerritotry;
    [field: SerializeField] public Transform CitizenPlace { get; private set; }

    private List<CitizenController> houseOccupants = new ();

    private List<HouseRepairable> repairables = new ();
    private bool isBroken;

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

    public void AddRepairable(HouseRepairable repairable)
    {
        repairables.Add(repairable);
    }
    
    public bool CanRepair(IRepairer repairer)
    {
        return CanRepair(repairer, out IRepairable target);
    }

    public void AddCitizen(CitizenController citizen)
    {
        var pos = GetRandomPositionOnHouseTerritory();
        citizen.Place(pos);
        houseOccupants.Add(citizen);
    }

    public int GetCitizensCapacity()
    {
        var count = 0;
        for (int i = 0; i < gameConfig.CitizenCountsForHouseLevels.Length; i++)
        {
            if (Level > i)
            {
                count = gameConfig.CitizenCountsForHouseLevels[i];
            }
        }
        return count;
    }
    
    public int GetCitizensCount()
    {
        return houseOccupants.Count;
    }
    
    private Vector3 GetRandomPositionOnHouseTerritory()
    {
        var box = houseTerritotry.GetRandom();
        var min = box.transform.position - box.transform.TransformVector(box.size / 2f);
        var max = box.transform.position + box.transform.TransformVector(box.size / 2f);
        if (min.x > max.x)
        {
            var tmp = min.x;
            min.x = max.x;
            max.x = tmp;
        }
        if (min.y > max.y)
        {
            var tmp = min.y;
            min.y = max.y;
            max.y = tmp;
        }
        if (min.z > max.z)
        {
            var tmp = min.z;
            min.z = max.z;
            max.z = tmp;
        }
        
        var randPos = new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z));
        return randPos;
    }
}
