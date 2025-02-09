using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WishGrantersManager : MonoBehaviour
{
    [Inject] private WishesConfig wishesConfig;
    
    private Dictionary<Type, List<WishGranter>> grantersDict = new ();

    public void AddGranter(WishGranter granter)
    {
        var type = granter.GetType();
        if (!grantersDict.ContainsKey(type))
        {
            grantersDict[type] = new List<WishGranter>();
        }

        grantersDict[type].Add(granter);
    }
    
    public WishGranter TryGetWorkingFreeGranter()
    {
        WishGranter granter = null;
        var type = wishesConfig.GetRandomWishType();
        foreach (var granterCandidate in grantersDict[type])
        {
            if (granterCandidate.IsWorking() && granterCandidate.CanAddOneMore())
            {
                granter = granterCandidate;
                break;
            }   
        }

        return granter;
    }
}