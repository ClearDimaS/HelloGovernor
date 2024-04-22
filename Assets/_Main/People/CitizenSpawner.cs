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

    [SerializeField] private CitizenController[] citizenPrefabs;
    [SerializeField] private int baseCount;

    private List<CitizenController> citizens = new ();
    private Dictionary<HouseBuilding, List<CitizenController>> citizensDict = new ();

    private void Update()
    {
        SpawnNew();
    }

    private void SpawnNew()
    {
        for (int i = citizens.Count; i < baseCount; i++)
        {
            var citizen = Spawn();
            citizen.PlaceRandom();
        }

        foreach (var house in housesManager.Houses)
        {
            if (!citizensDict.ContainsKey(house))
            {
                citizensDict[house] = new ();
            }

            if (citizensDict[house].Count < house.CitizensCount)
            {
                var old = citizensDict[house].Count;
                for (int i = 0; i < house.CitizensCount - old; i++)
                {
                    CitizenController citizen = Spawn();
                    citizensDict[house].Add(citizen);
                    //citizen.Place(house.CitizenPlace.position);
                    citizen.PlaceRandom();
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
