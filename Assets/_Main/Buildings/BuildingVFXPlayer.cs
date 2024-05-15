using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuildingVFXPlayer : MonoBehaviour
{
    [SerializeField] protected int psCount = 3;
    [SerializeField] protected ParticleSystem[] upgradePS;
    
    private UpgradableBuilding building;

    private void Awake()
    {
        building = GetComponentInParent<UpgradableBuilding>();
        building.SubscribeUpgrade(PlayUpgradePS);
    }

    private void PlayUpgradePS()
    {
        var offsetIndex = Random.Range(0, upgradePS.Length);
        var stepIndex = Random.Range(1, 3);
        for (int i = 0; i < psCount; i++)
        {
            upgradePS[(offsetIndex + i * stepIndex) % upgradePS.Length].Play();
        }
    }
}
