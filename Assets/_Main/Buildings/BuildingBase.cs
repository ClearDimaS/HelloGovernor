using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] private UpgradableBuilding upgradable;
    
    public bool IsBought => upgradable.IsBought;

    private void Awake()
    {
        if (upgradable == null)
        {
            upgradable = GetComponent<UpgradableBuilding>();
        }
        OnAwake();
    }

    private void Update()
    {
        if (IsBought)
        {
            OnUpdate();
        }
    }
    
    protected virtual void OnAwake()
    {
        
    }
    
    protected virtual void OnUpdate()
    {
        
    }
}
