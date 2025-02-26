using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TrainStationWishGranter : OperatableGranter
{
    [Inject] protected CitizenSpawner spawner;
    
    [SerializeField] protected Transform[] waitPlaces;
    [SerializeField] protected float maxTimeToWalk = 3f;
    protected List<CitizenController> waitingCitizens = new ();
    
    protected TownHallWishGranter townHallWishGranter;
    
    protected override void OnAwake()
    {
        base.OnAwake();
        townHallWishGranter = FindObjectOfType<TownHallWishGranter>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (TrainBehaviour.Instance.GetState() == ETrainState.Departure && TrainBehaviour.Instance.GetTimeLeft() > maxTimeToWalk)
        {
            foreach (var waiting in waitingCitizens)
            {
                if (!waiting.Walker.IsMoving)
                {
                    var entry = TrainBehaviour.Instance.GetNearestEntry(waiting.transform);
                    var tmp = waiting;
                    waiting.Walker.MoveToTarget(entry.position, () =>
                    {
                        waitingCitizens.Remove(tmp);
                        spawner.Pool(tmp);
                    });
                }
            }   
        }
    }

    public override bool CanAddOneMore()
    {
        return base.CanAddOneMore() && townHallWishGranter.HasAnyExtra;
    }

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return base.CanAddProgress(citizen) && waitPlaces.Length > waitingCitizens.Count;
    }

    protected override void OnLeave(CitizenController citizen)
    {
        base.OnLeave(citizen);
        townHallWishGranter.RemoveExtraCitizen();
        citizen.WishesController.DisableAllDesires();
        citizen.Walker.MoveToTarget(waitPlaces[waitingCitizens.Count].position, () => {});
        waitingCitizens.Add(citizen);
    }
}
