using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TownHallWishGranter : OperatableGranter
{
    [Inject] protected CitizenSpawner spawner;

    [SerializeField] protected float exitFromTrainPause = 0.2f;

    protected float lastExitTime;
    protected int extraCitizensCount;
    public bool HasAnyExtra => extraCitizensCount > 0;

    public override bool CanAddOneMore()
    {
        return base.CanAddOneMore() && extraCitizensCount + QueueBusyCount < QueueMaxCount;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (TrainBehaviour.Instance.GetState() == ETrainState.Arrival && Time.time > 10f)
        {
            if (IsWorking() && CanAddOneMore() && Time.time - lastExitTime > exitFromTrainPause)
            {
                lastExitTime = Time.time;
                var citizen = spawner.GetNewCitizen();
                var entry = TrainBehaviour.Instance.GetRandomEntry();
                citizen.Place(entry.position);
                citizen.WishesController.SetWish(this);
                extraCitizensCount++;
            }   
        }
    }

    public void RemoveExtraCitizen()
    {
        extraCitizensCount--;
    }
}
