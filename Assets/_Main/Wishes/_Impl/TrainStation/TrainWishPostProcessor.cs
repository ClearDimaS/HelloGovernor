using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TrainWishPostProcessor : WishPostProcessor
{
    [Inject] protected CitizenSpawner spawner;

    [SerializeField] protected string waitingAnimation = "Sit";
    [SerializeField] protected Transform[] waitPlaces;
    [SerializeField] protected float maxTimeToWalk = 3f;
    protected List<CitizenController> waitingCitizens = new ();
    
    protected TownHallWishGranter townHallWishGranter;
    
    protected void Awake()
    {
        townHallWishGranter = FindObjectOfType<TownHallWishGranter>();
    }

    public override void OnUpdate()
    {
        if (TrainBehaviour.Instance.GetState() == ETrainState.Departure && TrainBehaviour.Instance.GetTimeLeft() > maxTimeToWalk)
        {
            foreach (var waiting in waitingCitizens)
            {
                if (!waiting.Walker.IsMoving)
                {
                    waiting.ResetAnimation();
                    var entry = TrainBehaviour.Instance.GetNearestEntry(waiting.transform);
                    var tmp = waiting;
                    waiting.Walker.MoveToTarget(entry.position, () =>
                    {
                        Remove(tmp);
                    });
                }
            }   
        }
    }

    public override bool HasMorePlace()
    {
        return waitingCitizens.Count < waitPlaces.Length;
    }

    public override void Add(CitizenController citizen)
    {
        citizen.WishesController.DisableAllDesires();
        citizen.Walker.MoveToTarget(waitPlaces[waitingCitizens.Count].position, () => citizen.PlayAnimation(waitingAnimation));
        waitingCitizens.Add(citizen);
    }

    private void Remove(CitizenController citizen)
    {
        townHallWishGranter.RemoveExtraCitizen();
        waitingCitizens.Remove(citizen);
        spawner.Pool(citizen);
    }
}