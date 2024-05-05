using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class WishGranter : MonoBehaviour
{
    [Inject] private WishGrantersManager grantersManager;

    [SerializeField] private CurrencyStackBehaviour currencyStack;
    [SerializeField] private Transform[] processPlaces;
    [SerializeField] private Transform exit;
    [SerializeField] private UpgradableObject upgradable;
    public abstract EWish Type { get; }
    public abstract float FullProgressTime { get; }
    public abstract int Reward { get; }

    private List<CitizenController> approaching = new ();
    private List<CitizenController> pendingQueue = new ();
    private List<CitizenController> queue = new ();
    private List<CitizenController> pendingProcessed = new ();
    protected List<CitizenController> processed = new ();
    private List<CitizenController> pendingLeaving = new ();
    private List<CitizenController> leaving = new ();
    protected List<CitizenController> pendingRemove = new ();

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
        // 1. Approqch
        foreach (var citizen in approaching)
        {
            var queuePlace = GetQueuePlaceFor(citizen);
            citizen.Walker.MoveToTarget(queuePlace, () => pendingQueue.Add(citizen));
        }
        foreach (var citizen in pendingQueue)
        {
            AddToQueue(citizen);
        }
        pendingQueue.Clear();
        
        // 2. In queue
        foreach (var citizen in queue)
        {
            var processPlace = GetProcessPlaceFor(citizen);
            citizen.Walker.MoveToTarget(processPlace, () => pendingProcessed.Add(citizen));
        }
        foreach (var citizen in pendingProcessed)
        {
            AddToProcessed(citizen);
        }
        pendingProcessed.Clear();

        // 3. Processed
        foreach (var citizen in processed)
        {
            UpdateProcessed(citizen);
            if (citizen.WishesController.IsProgressFull(Type))
            {
                pendingLeaving.Add(citizen);
            }
        }
        foreach (var citizen in pendingLeaving)
        {
            OnSuccessProcess(citizen);
            AddToLeaving(citizen);
        }
        pendingLeaving.Clear();

        // 4. Leaving
        foreach (var citizen in leaving)
        {
            var exitPlace = GetExitPlaceFor(citizen);
            citizen.Walker.MoveToTarget(exitPlace, () => pendingRemove.Add(citizen));
        }

        foreach (var citizen in pendingRemove)
        {
            RemoveFromLeaving(citizen);
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
        return upgradable == null || upgradable.IsBought;
    }

    public bool HasInQueueOrProcessed(CitizenController citizen)
    {
        return approaching.Contains(citizen) || queue.Contains(citizen) || processed.Contains(citizen) || leaving.Contains(citizen);
    }

    protected abstract bool CanAddProgress(CitizenController citizen);
    
    protected virtual Vector3 GetQueuePlaceFor(CitizenController citizen)
    {
        var index = queue.IndexOf(citizen);
        if (index < 0)
        {
            index = Mathf.Max(processed.Count - 1, 0);
        }

        return processPlaces[index % processPlaces.Length].position;
    }
    
    protected virtual Vector3 GetProcessPlaceFor(CitizenController citizen)
    {
        var index = processed.IndexOf(citizen);
        if (index < 0)
        {
            index = Mathf.Max(processed.Count - 1, 0);
        }
        return processPlaces[index % processPlaces.Length].position;
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
        OnRemoveFromProcessed(citizen);
        processed.Remove(citizen);
        leaving.Add(citizen);
    }

    private void RemoveFromLeaving(CitizenController citizen)
    {
        leaving.Remove(citizen);
    }

    public void Abort(CitizenController citizen)
    {
        approaching.Remove(citizen);
        queue.Remove(citizen);
        OnRemoveFromProcessed(citizen);
        processed.Remove(citizen);
        leaving.Remove(citizen);
    }

    protected virtual void OnRemoveFromProcessed(CitizenController citizen)
    {
        
    }

    protected virtual void OnSuccessProcess(CitizenController citizen)
    {
        if (currencyStack != null)
        {
            currencyStack.MoveCurrencyToMe(Reward, citizen.transform.position);   
        }
    }

    public virtual bool CanAddOneMore()
    {
        return approaching.Count + queue.Count + processed.Count < processPlaces.Length;
    }

    public bool CanAdd(CitizenController citizenController)
    {
        return !citizenController.Interactor.HasItem(Type);
    }

    public bool IsProcessed(CitizenController target)
    {
        return processed.Contains(target);
    }

    public CitizenController GetProcessedWithoutAssistant()
    {
        foreach (var citizen in processed)
        {
            if (!citizen.WishesController.IsGranterAssistantServing(Type))
            {
                return citizen;
            }
        }

        return null;
    }
}