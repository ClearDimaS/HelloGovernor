using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public abstract class WishGranter : MonoBehaviour
{
    [Inject] protected WishesConfig wishesConfig;
    [Inject] private WishGrantersManager grantersManager;
    [Inject] protected CompassManager compassManager;

    [SerializeField] private CurrencyStackBehaviour currencyStack;
    [SerializeField] private Transform[] queuePlaces;
    [SerializeField] private WishPlace[] processPlaces;
    [SerializeField] private Transform exit;
    [SerializeField] private UpgradableObject upgradable;
    [field: SerializeField] public bool NeedItems { get; set; }
    
    private List<CitizenController> approaching = new ();
    private List<CitizenController> pendingQueue = new ();
    private List<CitizenController> queue = new ();
    private Dictionary<CitizenController, WishPlace> citizenPlacesDict = new ();
    private Queue<WishPlace> freePlaces = new ();
    protected List<CitizenController> processed = new ();
    private List<CitizenController> pendingLeaving = new ();
    private List<CitizenController> leaving = new ();
    protected List<CitizenController> pendingRemove = new ();
    protected WishPlace[] WishPlaces => processPlaces;

    public float FullProgressTime => wishesConfig.GetGrantDuration(this);
    public int Reward => wishesConfig.GetReward(this);

    private void Awake()
    {
        grantersManager.AddGranter(this);
        for (int i = 0; i < processPlaces.Length; i++)
        {
            freePlaces.Enqueue(processPlaces[i]);
        }
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        
    }

    private void Update()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            if (queuePlaces.Length > 0)
            {
                if (queuePlaces.Length <= i)
                {
                    continue;
                }
                var citizen = queue[i];
                var queuePlace = queuePlaces[i];
                if ((citizen.transform.position - queuePlace.transform.position).magnitude > 0.2f && !citizen.Walker.IsMoving)
                {
                    citizen.Walker.MoveToTarget(queuePlace.position, () =>
                    {
                        citizen.transform.DORotateQuaternion(queuePlace.transform.rotation, 0.15f);
                    });   
                }
            }
            else
            {
                
            }
        }
        // 1. Approqch
        foreach (var citizen in approaching)
        {
            var queuePlace = GetQueuePlaceFor(citizen);
            if (!citizen.Walker.IsMoving)
            {
                citizen.Walker.MoveToTarget(queuePlace, () => pendingQueue.Add(citizen));
            }
        }
        foreach (var citizen in pendingQueue)
        {
            AddToQueue(citizen);
        }
        pendingQueue.Clear();
        
        // 2. In queue
        for (int i = 0; i < queue.Count; i++)
        {
            var citizen = queue[i];
            if (processPlaces.Length > 0)
            {
                if (freePlaces.Count > 0)
                {
                    citizenPlacesDict[citizen] = DequeuePlace();   
                    citizenPlacesDict[citizen].SetOwner(citizen);
                    var processPlace = GetProcessPlaceFor(citizen);
                    queue.RemoveAt(i);
                    i--;
                    citizen.Walker.MoveToTarget(processPlace, () =>
                    {
                        AddToProcessed(citizen);
                    });   
                }
            }
            else
            {
                queue.RemoveAt(i);
                i--;
                AddToProcessed(citizen);
            }
        }

        // 3. Processed
        foreach (var citizen in processed)
        {
            var wasReady = citizen.WishesController.IsProgressFull(this);
            UpdateProcessed(citizen);
            if (citizen.WishesController.IsProgressFull(this) && wasReady)
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
        pendingRemove.Clear();

        OnUpdate();
    }
    
    protected virtual void OnUpdate()
    {
        
    }
    
    protected virtual void UpdateProcessed(CitizenController citizen)
    {
        if (CanAddProgress(citizen))
        {
            citizen.WishesController.AddProgress(this, Time.deltaTime / FullProgressTime);   
        }
    }

    public virtual bool IsWorking()
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

        var indexAll = queue.Count + processed.Count + approaching.Count - 1;
        if (processPlaces.Length <= indexAll && queuePlaces.Length > 0)
        {
            return queuePlaces[index % queuePlaces.Length].position;
        }
        return processPlaces[index % processPlaces.Length].Position;
    }
    
    protected virtual Vector3 GetProcessPlaceFor(CitizenController citizen)
    {
        return GetProcessRootFor(citizen).position;
    }
    
    protected virtual Quaternion GetProcessRotFor(CitizenController citizen)
    {
        return GetProcessRootFor(citizen).rotation;
    }
    
    private Transform GetProcessRootFor(CitizenController citizen)
    {
        return citizenPlacesDict[citizen].transform;
    }
    
    protected virtual Vector3 GetExitPlaceFor(CitizenController citizen)
    {
        return exit.position;
    }
    
    public void AddApproaching(CitizenController citizen)
    {
        if (CanAddOneMore())
        {
            if (!approaching.Contains(citizen))
            {
                approaching.Add(citizen);
            }
        }
        else
        {
            Debug.LogError($"cant add approaching!  {this}");   
        }
    }

    private void AddToQueue(CitizenController citizen)
    {
        if (approaching.Remove(citizen))
        {
            queue.Add(citizen);
        }
    }

    private void AddToProcessed(CitizenController citizen)
    {
        if (citizenPlacesDict.ContainsKey(citizen))
        {
            citizenPlacesDict[citizen].TakePlace(citizen);   
        }
        if (!processed.Contains(citizen))
        {
            processed.Add(citizen);   
        }
        var place = GetProcessPlaceFor(citizen);
        var rot = GetProcessRotFor(citizen);
        citizen.transform.DORotateQuaternion(rot, 0.3f);
        citizen.transform.DOMove(place, 0.3f);
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
        if (citizenPlacesDict.ContainsKey(citizen))
        {
            citizenPlacesDict[citizen].LeavePlace(citizen);
            EnqueuePlace(citizenPlacesDict[citizen]);
            citizenPlacesDict.Remove(citizen);   
        }
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
        return approaching.Count + queue.Count * processed.Count < queuePlaces.Length + processPlaces.Length;
    }

    private WishPlace DequeuePlace()
    {
        var place = freePlaces.Dequeue();
        return place;
    }
    
    private void EnqueuePlace(WishPlace place)
    {
        freePlaces.Enqueue(place);
    }

    public bool CanAdd(CitizenController citizenController)
    {
        return !citizenController.HasItem(this);
    }

    public bool IsProcessed(CitizenController target)
    {
        return processed.Contains(target);
    }

    public CitizenController GetProcessedWithoutAssistant()
    {
        foreach (var citizen in processed)
        {
            if (!citizen.WishesController.IsGranterAssistantServing(this))
            {
                return citizen;
            }
        }

        return null;
    }
}