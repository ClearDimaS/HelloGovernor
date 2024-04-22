using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousesManager : MonoBehaviour
{
    private List<HouseBuilding> houses = new ();
    public IEnumerable<HouseBuilding> Houses => houses;

    public void Add(HouseBuilding houseBuilding)
    {
        houses.Add(houseBuilding);
    }
}