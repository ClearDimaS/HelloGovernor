using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class ThiefBehaviour : MonoBehaviour, IResetable
{
    [Inject] private EnvironmentManager environmentManager;
    
    [SerializeField] private TimerBase catchTimer;
    [SerializeField] private Walker walker;
    
    public void OnReset()
    {
        catchTimer.SetProgress(0f);
    }

    public void OnPool()
    {

    }

    private void Update()
    {
        if (!walker.IsMoving)
        {
            walker.MoveToTarget(environmentManager.GetRandomUnlockedPosition(0), null);
        }
    }

    public void SetCatchProgress(float catchProgress)
    {
        catchTimer.SetProgress(catchProgress);
    }
}
