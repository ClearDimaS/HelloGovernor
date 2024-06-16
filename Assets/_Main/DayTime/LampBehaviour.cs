using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LampBehaviour : MonoBehaviour
{
    [Inject] private DayTimeManager dayTimeManager;

    [SerializeField] private GameObject[] enableGOs;

    private void Update()
    {
        foreach (var go in enableGOs)
        {
            if (go.activeSelf != dayTimeManager.IsLampsEnabled)
            {
                go.SetActive(dayTimeManager.IsLampsEnabled);
            }
        }
    }
}
