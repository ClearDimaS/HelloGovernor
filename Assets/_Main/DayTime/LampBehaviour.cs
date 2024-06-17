using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LampBehaviour : CulledBehaviour
{
    [Inject] private DayTimeManager dayTimeManager;

    [SerializeField] private GameObject[] enableGOs;

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        foreach (var go in enableGOs)
        {
            if (go.activeSelf != dayTimeManager.IsLampsEnabled)
            {
                go.SetActive(dayTimeManager.IsLampsEnabled);
            }
        }
    }
}
