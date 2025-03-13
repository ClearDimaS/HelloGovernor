using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingBase : CulledBehaviour
{
    [SerializeField] private UpgradableBuilding upgradable;
    
    public bool IsBought => upgradable.IsBought;
    public int Level => upgradable.Level;

    protected override void OnAwake()
    {
        base.OnAwake();
        if (upgradable == null)
        {
            upgradable = GetComponent<UpgradableBuilding>();
        }
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (IsBought)
        {
            OnUpdateBought(visible);
        }
    }

    protected virtual void OnUpdateBought(bool visible)
    {
        
    }

    public virtual void OnUpgrade()
    {

    }
}
