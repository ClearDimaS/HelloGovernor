using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LightChainBehaviour : CulledBehaviour
{
    [Inject] private DayTimeManager dayTimeManager;
    
    [SerializeField] private UpgradableObject[] objects;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject nightContent;

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        var isEnabled = true;
        foreach (var obj in objects)
        {
            if (!obj.IsBought)
            {
                isEnabled = false;
            }
        }

        var isDayEnabled = isEnabled && !dayTimeManager.IsLampsEnabled;
        var isNightEnabled = isEnabled && dayTimeManager.IsLampsEnabled;
        if (content.activeSelf != isDayEnabled )
        {
            content.SetActive(isDayEnabled);
        }

        if (nightContent.activeSelf != isNightEnabled)
        {
            nightContent.SetActive(isNightEnabled);
        }
    }
}
