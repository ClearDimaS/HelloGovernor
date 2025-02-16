using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class CitizenSpawner : MonoBehaviour
{
    [Inject] private DiContainer container;
    [Inject] private HousesManager housesManager;
    [Inject] private UpgradablePricesManager pricesManager;
    
    [SerializeField] private CitizenController[] citizenPrefabs;
    [SerializeField] private int baseCount;

    private List<CitizenController> citizens = new ();

    private void Update()
    {
        SpawnNew();
    }

    private void SpawnNew()
    {
        var citizensCount = baseCount;
        var boughtBuildings = pricesManager.GetBoughtGranters();
        foreach (var bought in boughtBuildings)
        {
            citizensCount += bought.GetCitizensCount();
        }
        
        for (int i = citizens.Count; i < citizensCount; i++)
        {
            var citizen = Spawn();
            citizen.PlaceRandom();
        }

        foreach (var house in housesManager.Houses)
        {
            var count = house.GetCitizensCount();
            var capacity = house.GetCitizensCapacity();
            if (count < capacity)
            {
                var old = count;
                for (int i = 0; i < house.GetCitizensCapacity() - old; i++)
                {
                    CitizenController citizen = Spawn();
                    house.AddCitizen(citizen);
                }
            }
        }
    }

    private CitizenController Spawn()
    {
        var prefab = citizenPrefabs[Random.Range(0, citizenPrefabs.Length)];
        var citizen = container.InstantiatePrefabForComponent<CitizenController>(prefab);
        citizens.Add(citizen);
        return citizen;
    }
}
