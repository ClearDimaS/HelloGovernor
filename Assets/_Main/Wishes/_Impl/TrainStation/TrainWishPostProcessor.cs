using System.Collections.Generic;
using DG.Tweening;
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

    public override bool IsProcessing(CitizenController citizen)
    {
        return waitingCitizens.Contains(citizen);
    }

    public override void OnUpdate()
    {
        if (TrainBehaviour.Instance.GetState() == ETrainState.Departure && TrainBehaviour.Instance.GetTimeLeft() > maxTimeToWalk)
        {
            for (var i = 0; i < waitingCitizens.Count; i++)
            {
                var waiting = waitingCitizens[i];
                if (!waiting.Walker.IsMoving && waiting.gameObject.activeSelf)
                {
                    waiting.ResetAnimation();
                    var entry = TrainBehaviour.Instance.GetNearestEntry(waiting.transform);
                    var tmp = waiting;
                    waiting.Walker.MoveToTarget(entry.position, () => { Remove(tmp); });
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
        var place = waitPlaces[waitingCitizens.Count];
        citizen.Walker.MoveToTarget(place.position, () =>
        {
            citizen.transform.DORotateQuaternion(place.rotation, 0.2f);
            citizen.PlayAnimation(waitingAnimation);
        });
        waitingCitizens.Add(citizen);
    }

    private void Remove(CitizenController citizen)
    {
        townHallWishGranter.RemoveExtraCitizen();
        waitingCitizens.Remove(citizen);
        spawner.Pool(citizen);
    }
}