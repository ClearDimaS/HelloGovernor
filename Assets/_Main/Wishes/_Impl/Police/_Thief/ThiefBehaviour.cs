using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class ThiefBehaviour : MonoBehaviour, IResetable
{
    [SerializeField] private TimerBase catchTimer;
    
    public void OnReset()
    {
        catchTimer.SetProgress(0f);
    }

    public void OnPool()
    {

    }

    public void SetCatchProgress(float catchProgress)
    {
        catchTimer.SetProgress(catchProgress);
    }
}
