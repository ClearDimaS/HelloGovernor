using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HousesManager : MonoBehaviour
{
    [Inject] private GameConfig gameConfig;
    
    private float timeBeforeBreak = 0f;
    private float breakTimer;
    private List<HouseBuilding> houses = new ();
    
    public IEnumerable<HouseBuilding> Houses => houses;
    private Vector2 breakTimerMinMax => gameConfig.breakTimerMinMax;

    private void Start()
    {
        ReinitBreakTimer();
    }

    private void Update()
    {
        breakTimer += Time.deltaTime;
        if (breakTimer > timeBeforeBreak)
        {
            BreakBuilding();
            ReinitBreakTimer();
        }
    }

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
    
    private void BreakBuilding()
    {
        houses.Shuffle();
        foreach (var house in houses)
        {
            if (!house.IsBroken() && house.IsBought)
            {
                house.Break();
                break;
            }
        }
    }
    
    private void ReinitBreakTimer()
    {
        breakTimer = 0f;
        timeBeforeBreak = Random.Range(breakTimerMinMax.x, breakTimerMinMax.y);
    }
}