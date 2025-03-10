using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WishGrantersManager : MonoBehaviour
{
    [Inject] private WishesCollectionConfig wishesCollectionConfig;

    [SerializeField] protected GameObject moneyShowerPrefab;
    private Dictionary<Type, List<WishGranter>> grantersDict = new ();

    private Dictionary<WishGranter, UpgradableBuilding> possibleShowersDict = new ();
    private HashSet<WishGranter> moneyShowersAdded = new ();

    private void Start()
    {
        StartCoroutine(SpawnShowers());
    }

    private IEnumerator SpawnShowers()
    {
        while (true)
        {
            foreach (var granterPair in possibleShowersDict)
            {
                if (granterPair.Value.Level > 1 && !moneyShowersAdded.Contains(granterPair.Key))
                {
                    moneyShowersAdded.Add(granterPair.Key);
                    var shower = Instantiate(moneyShowerPrefab, granterPair.Key.transform);
                    shower.transform.position = granterPair.Key.GetMiddleProcessPlace();
                }
                yield return null;   
            }
        }
    }

    public void AddGranter(WishGranter granter)
    {
        var type = granter.GetType();
        if (!grantersDict.ContainsKey(type))
        {
            grantersDict[type] = new List<WishGranter>();
        }

        var building = granter.GetComponent<UpgradableBuilding>();
        if (building != null && building.LevelsCount > 1)
        {
            possibleShowersDict[granter] = building;
        }
        grantersDict[type].Add(granter);
    }
    
    public WishGranter TryGetWorkingFreeGranter()
    {
        WishGranter granter = null;
        var type = wishesCollectionConfig.GetRandomWishType();
        foreach (var granterCandidate in grantersDict[type])
        {
            if (granterCandidate.IsWorking() && granterCandidate.CanAddOneMore() && granterCandidate.Weight > 0)
            {
                granter = granterCandidate;
                break;
            }   
        }

        return granter;
    }
}