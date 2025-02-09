using System;
using System.Collections.Generic;
using UnityEngine;

public class WishGrantersManager : MonoBehaviour
{
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
    
    public bool TryGetWorkingFreeGranter(Type type, out WishGranter granter)
    {
        if (!grantersDict.ContainsKey(type) || grantersDict[type].Count == 0)
        {
            granter = null;
            return false;
        }

        foreach (var granterCandidate in grantersDict[type])
        {
            if (granterCandidate.IsWorking() && granterCandidate.CanAddOneMore())
            {
                granter = granterCandidate;
                return true;
            }
        }

        granter = null;
        return false;
    }
}