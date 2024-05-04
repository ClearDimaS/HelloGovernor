using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CopAssistant : MonoBehaviour, IThiefBuster
{
    [Inject] private ThiefsManager thiefsManager;

    [SerializeField] private Walker walker;
    
    private PolicestationBuilding policeStation;

    private void Awake()
    {
        policeStation = GetComponentInParent<PolicestationBuilding>();
    }

    private void Update()
    {
        if (thiefsManager.IsStealing)
        {
            var thief = thiefsManager.GetStealer();
            if (thief.IsStealing)
            {
                walker.MoveToTarget(thief.transform.position, null);
                return;
            }
        }

        walker.MoveToTarget(policeStation.PolicePlace.position, null);
    }
}
