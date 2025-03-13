using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class WanderWishGranter : TimerWishGranter
{
    [Inject] private EnvironmentManager environment;

    public override bool ProcessInstant => true;
    protected IEnumerator cor;
    protected int lastReshuffleIndex;

    protected override void OnStart()
    {
        base.OnStart();
        cor = ReshuffleCoroutine();
        Reshuffle();
    }

    protected override void OnRemoveFromProcessed(CitizenController citizen)
    {
        base.OnRemoveFromProcessed(citizen);
        if (lastReshuffleIndex > processPlaces.Length)
        {
            lastReshuffleIndex = -1;
            Reshuffle(lastReshuffleIndex);
        }
        else
        {
            Reshuffle(lastReshuffleIndex++);
        }
    }

    protected override void UpdateProcessed(CitizenController citizen)
    {
        base.UpdateProcessed(citizen);
        if (!citizen.Walker.IsMoving)
        {
            var pos = GetRandomPos();
            citizen.Walker.MoveToTarget(pos, null);
        }
    }
    
    private Vector3 GetRandomPos()
    {
        return environment.GetRandomUnlockedPosition(0f);
    }
    
    private void Reshuffle(int index=-1)
    {
        if (!environment.IsReady)
        {
            UniTask.WaitUntil(() => environment.IsReady).ContinueWith(() =>
            {
                Reshuffle();
            });
            return;
        }

        if (index == -1)
        {
            StartCoroutine(cor);
        }
        else
        {
            for (var i = 0; i < processPlaces.Length; i++)
            {
                if (index != -1 && index != i)
                {
                    continue;
                }
                var processPlace = processPlaces[i];
                processPlace.transform.position = environment.GetRandomUnlockedPosition(0);
                processPlace.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
            }
        }
        
    }

    private IEnumerator ReshuffleCoroutine()
    {
        for (var i = 0; i < processPlaces.Length; i++)
        {
            var processPlace = processPlaces[i];
            processPlace.transform.position = environment.GetRandomUnlockedPosition(0);
            processPlace.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
            yield return null;
        }
    }
}