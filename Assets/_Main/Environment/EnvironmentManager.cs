using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class EnvironmentManager : MonoBehaviour
{
    [Inject] private UpgradablePricesManager upgradableManager;
    [SerializeField] private Transform center;
    [SerializeField] private BoxCollider sizeCollider;

    public Vector3 mapSize => sizeCollider.size;
    public Vector3 mapCenter => center.position;
    public bool IsReady => upgradableManager.GetBoughtBuildings().Count > 0;

    public Vector3 GetRandomUnlockedPosition(float safetyRadius)
    {
        var boughtHouses = upgradableManager.GetBoughtBuildings();
        return boughtHouses[Random.Range(0, boughtHouses.Count - 1)].GetRandomPosition();
    }
}
