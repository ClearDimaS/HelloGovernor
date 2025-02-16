using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HousesManager : MonoBehaviour
{
    private List<HouseBuilding> houses = new ();
    
    public IEnumerable<HouseBuilding> Houses => houses;

    public void Add(HouseBuilding houseBuilding)
    {
        houses.Add(houseBuilding);
    }
    
    public HouseBuilding GetBrokenBuildingForRepair()
    {
        foreach (var house in houses)
        {
            if (house.CanRepair(null, out IRepairable target))
            {
                return house;
            }
        }

        return null;
    }
}