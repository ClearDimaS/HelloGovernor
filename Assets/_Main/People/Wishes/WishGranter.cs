using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WanderWishGranter : WishGranter
{
    [Inject] private GameConfig gameConfig;
    
    public override EWish Type => EWish.Wander;
    public override float FullProgressTime => gameConfig.wanderDuration;

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return true;
    }

    protected override Vector3 GetQueuePlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }

    protected override Vector3 GetProcessPlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }

    protected override Vector3 GetExitPlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }
}

public class ChatWishGranter : WishGranter
{
    [Inject] private GameConfig gameConfig;
    
    public override EWish Type => EWish.Chat;
    public override float FullProgressTime => gameConfig.chatDuration;
    
    protected override Vector3 GetQueuePlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }

    protected override Vector3 GetProcessPlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }

    protected override Vector3 GetExitPlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }
    
    protected override bool CanAddProgress(CitizenController citizen)
    {
        return citizen.IsChatting();
    }
}

public abstract class WishGranter : MonoBehaviour
{
    [Inject] private WishGrantersManager grantersManager;

    [SerializeField] private Transform[] processPlaces;
    [SerializeField] private Transform exit;
    [SerializeField] private UpgradableObject upgradable;
    public abstract EWish Type { get; }
    public abstract float FullProgressTime { get; }

    private List<CitizenController> approaching = new ();
    private List<CitizenController> queue = new ();
    private List<CitizenController> processed = new ();
    private List<CitizenController> leaving = new ();

    private void Awake()
    {
        grantersManager.AddGranter(this);
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        
    }

    private void Update()
    {
        foreach (var citizen in approaching)
        {
            citizen.Walker.MoveToTarget(GetQueuePlaceFor(citizen), () => AddToQueue(citizen));
        }
        foreach (var citizen in queue)
        {
            citizen.Walker.MoveToTarget(GetProcessPlaceFor(citizen), () => AddToProcessed(citizen));
        }

        foreach (var citizen in processed)
        {
            UpdateProcessed(citizen);
            if (citizen.WishesController.IsProgressFull(Type))
            {
                AddToLeaving(citizen);
            }
        }

        foreach (var citizen in leaving)
        {
            var exitPlace = GetExitPlaceFor(citizen);
            citizen.Walker.MoveToTarget(exitPlace, () => RemoveFromLeaving(citizen));
        }

        OnUpdate();
    }
    
    protected virtual void OnUpdate()
    {
        
    }
    
    protected virtual void UpdateProcessed(CitizenController citizen)
    {
        if (CanAddProgress(citizen))
        {
            citizen.WishesController.AddProgress(Type, Time.deltaTime / FullProgressTime);   
        }
    }

    public bool IsWorking()
    {
        return upgradable.IsBought;
    }

    public bool HasInQueueOrProcessed(CitizenController citizen)
    {
        return approaching.Contains(citizen) || queue.Contains(citizen) || processed.Contains(citizen) || leaving.Contains(citizen);
    }

    protected abstract bool CanAddProgress(CitizenController citizen);
    
    protected virtual Vector3 GetQueuePlaceFor(CitizenController citizen)
    {
        return processPlaces[0].position;
    }
    
    protected virtual Vector3 GetProcessPlaceFor(CitizenController citizen)
    {
        return processPlaces[0].position;
    }
    
    protected virtual Vector3 GetExitPlaceFor(CitizenController citizen)
    {
        return exit.position;
    }
    
    public void AddApproaching(CitizenController citizen)
    {
        approaching.Add(citizen);
    }

    private void AddToQueue(CitizenController citizen)
    {
        approaching.Remove(citizen);
        queue.Add(citizen);
    }

    private void AddToProcessed(CitizenController citizen)
    {
        queue.Remove(citizen);
        processed.Add(citizen);
    }

    private void AddToLeaving(CitizenController citizen)
    {
        processed.Remove(citizen);
        leaving.Add(citizen);
    }

    private void RemoveFromLeaving(CitizenController citizen)
    {
        leaving.Remove(citizen);
    }
}