using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public abstract class WishGranter<T, U> : WishGranter where T : WishGranterConfig where U : ProcessPlace
{
    protected U[] wishPlacesTyped;
    
    protected T config;

    public override float Weight => config.chanceWeight;

    protected override void OnConstruct()
    {
        base.OnConstruct();
        wishPlacesTyped = processPlaces.Select(x => x as U).ToArray();
        config = wishesCollectionConfig.GetConfig(this) as T;
        SetAnimationNames();
        if (config == null)
        {
            throw new NotImplementedException($"config of type {typeof(T)} is not added for granter {GetType()}");
        }
    }

    private void SetAnimationNames()
    {
        foreach (var processPlace in processPlaces)
        {
            processPlace.animationName = config.processAnimation;
        }

        foreach (var extraPlacesData in extraPlaces)
        {
            foreach (var processPlace in extraPlacesData.processPlaces)
            {
                processPlace.animationName = config.processAnimation;
            }
        }
    }

    protected override void AddExtraPlaces(WishGranterExtraPlacesData extraPlace)
    {
        base.AddExtraPlaces(extraPlace);
        wishPlacesTyped = processPlaces.Select(x => x as U).ToArray();
    }
}

public enum QueueExtraPlacesMode
{
    Sequential,
    Parallel
}
[Serializable]
public class WishGranterExtraPlacesData
{
    public ProcessPlace[] processPlaces;
    public Transform[] extraQueuePlaces;
    public QueueExtraPlacesMode queueMode = QueueExtraPlacesMode.Sequential;
}

public abstract class WishGranter : CulledBehaviour, ICooldownable
{
    [Inject] protected GameConfig _gameConfig;
    
    [SerializeField] protected WishPostProcessor wishPostProcessor;
    [Header("Optional Places")]
    [SerializeField] private Transform[] queuePlaces;
    [SerializeField] protected ProcessPlace[] processPlaces;

    //optional, crunchy 
    [Header("Optional Buildings")]
    [SerializeField, PropertyOrder(9999)] private Transform exit;    
    [SerializeField, PropertyOrder(9999)] private UpgradableObject upgradable;
    [SerializeField, PropertyOrder(9999)] private CurrencyStackBehaviour currencyStack;
    [SerializeField] protected WishGranterExtraPlacesData[] extraPlaces;
    private List<CitizenController> approaching = new ();
    private List<CitizenController> pendingQueue = new ();
    private List<CitizenController> queue = new ();
    private Dictionary<CitizenController, ProcessPlace> citizenPlacesDict = new ();
    private Queue<ProcessPlace> freePlaces = new ();
    protected List<CitizenController> processed = new ();
    private List<CitizenController> pendingLeaving = new ();
    private List<CitizenController> leaving = new ();
    protected List<CitizenController> pendingRemove = new ();
    private int extraPlacesAddedLevel = -1;
    public float FullProgressTime => wishesCollectionConfig.GetGrantDuration(this);
    public int Reward => wishesCollectionConfig.GetReward(this);
    public float CoolDown => coolDownTimer == null ? -1f : coolDownTimer.CoolDownTime;
    public float CoolDownTimeLeft => coolDownTimer == null ? -1f : coolDownTimer.CoolDownTimeLeft;
    public bool IsCooldown => coolDownTimer == null ? false : coolDownTimer.IsCooldown;
    public int ProcessedCounter { get; private set; }
    protected int QueueBusyCount => queue.Count;
    protected int QueueMaxCount => queuePlaces.Length;
    public float IncomeMultiplier { get; set; } = 1f;
    protected bool IsBought => upgradable == null || upgradable.IsBought;
    public abstract float Weight { get; }

    protected Action<Transform> onPlayerProcess;
    protected WishesCollectionConfig wishesCollectionConfig;
    private WishGrantersManager grantersManager;
    protected WishGranterCooldownTimer coolDownTimer;
    protected bool wasBought;
    public virtual bool ProcessInstant => false;

    [Inject]
    protected void Construct(WishesCollectionConfig wishesCollectionConfig, WishGrantersManager grantersManager)
    {
        this.wishesCollectionConfig = wishesCollectionConfig;
        this.grantersManager = grantersManager;
        OnConstruct();
        coolDownTimer = this.wishesCollectionConfig.GetCooldownTimer(this);
        Debug.Log($"{GetType()} timer is: {coolDownTimer}");
    }

    protected virtual void OnConstruct()
    {
        
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        foreach (var extraPlacesData in extraPlaces)
        {
            foreach (var processPlace in extraPlacesData.processPlaces)
            {
                processPlace.gameObject.SetActive(false);
            }
        }
        grantersManager.AddGranter(this);
        for (int i = 0; i < processPlaces.Length; i++)
        {
            freePlaces.Enqueue(processPlaces[i]);
        }
    }

    private void Start()
    {
        OnStart();
        wasBought = upgradable == null || upgradable.IsBought;
    }

    protected virtual void OnStart()
    {
        
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        #if UNITY_EDITOR
        foreach (var citizen in queue)
        {
            if (!citizen.WishesController.IsGranterMe(this))
            {
                Debug.LogError($"granter is not me!  {GetType()}    {transform.name}   {citizen.WishesController.GetGranter()}");
            }
        }
        #endif
        if (upgradable != null && upgradable.Level - 1 > extraPlacesAddedLevel)
        {
            extraPlacesAddedLevel = upgradable.Level - 1; // 1 for level 2
            var extraPlacesAddedIndex = extraPlacesAddedLevel - 1;  // 0 for level 2
            if (extraPlacesAddedIndex < extraPlaces.Length && extraPlacesAddedIndex >= 0)
            {
                AddExtraPlaces(extraPlaces[extraPlacesAddedIndex]);   
            }
        }
        if (coolDownTimer != null)
        {
            coolDownTimer.OnUpdate();
        }

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
            if (!citizen.Walker.IsMovingToTarget(queuePlace))
            {
                if (ProcessInstant)
                {
                    if (!pendingQueue.Contains(citizen))
                    {
                        pendingQueue.Add(citizen);
                    }
                }
                else
                {
                    citizen.Walker.MoveToTarget(queuePlace, () =>
                    {
                        if (!pendingQueue.Contains(citizen))
                        {
                            pendingQueue.Add(citizen);
                        }
                    });   
                }
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
                    if (!citizen.Walker.IsMoving)
                    {
                        citizenPlacesDict[citizen] = DequeuePlace();   
                        citizenPlacesDict[citizen].SetOwner(citizen);
                        var processPlace = GetProcessPlaceFor(citizen);
                        queue.RemoveAt(i);
                        i--;
                        citizen.Walker.MoveToTarget(processPlace, () =>
                        {
                            AddToProcessed(citizen);
                            citizenPlacesDict[citizen].SetAtPlace(citizen);
                        });   
                    }
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
            UpdateProcessed(citizen);
            var postProcessorCanLeave = wishPostProcessor == null ||
                                        wishPostProcessor.HasMorePlace();
            if (citizen.WishesController.IsProgressFull(this) && 
                postProcessorCanLeave)
            {
                ProcessedCounter++;
                pendingLeaving.Add(citizen);
                if (visible && IsPlayerProcessing(citizen))
                {
                    var place = citizen.transform;
                    onPlayerProcess?.Invoke(place);
                }
                OnLeave(citizen);
            }
        }
        foreach (var citizen in pendingLeaving)
        {
            OnSuccessProcess(citizen);
            AddToLeaving(citizen);
            if (wishPostProcessor != null)
            {
                wishPostProcessor.Add(citizen);
            }
        }
        pendingLeaving.Clear();

        // 4. Leaving
        foreach (var citizen in leaving)
        {
            if (wishPostProcessor != null)
            {
                if (!wishPostProcessor.IsProcessing(citizen))
                {
                    pendingRemove.Add(citizen);
                }
            }
            else
            {
                if (!citizen.Walker.IsMoving)
                {
                    var exitPlace = GetExitPlaceFor(citizen);
                    citizen.Walker.MoveToTarget(exitPlace, () => pendingRemove.Add(citizen));
                }   
            }
        }
        foreach (var citizen in pendingRemove)
        {
            RemoveFromLeaving(citizen);
        }
        pendingRemove.Clear();

        OnUpdate();
        if (wishPostProcessor != null)
        {
            wishPostProcessor.OnUpdate();
        }
    }

    protected abstract bool IsPlayerProcessing(CitizenController citizenController);


    protected void LeaveAllCitizens()
    {
        foreach (var citizen in queue)
        {
            citizen.WishesController.RemoveWish(this);
        }

        foreach (var citizen in processed)
        {
            citizen.WishesController.RemoveWish(this);
        }
        processed.Clear();
        queue.Clear();
    }

    public void SetReady()
    {
        coolDownTimer.SetGameReady();
    }

    protected virtual void AddExtraPlaces(WishGranterExtraPlacesData extraPlace)
    {
        foreach (var processPlace in extraPlace.processPlaces)
        {
            processPlace.gameObject.SetActive(true);
        }
        if (extraPlace.processPlaces.Length > 0)
        {
            processPlaces = processPlaces.Concat(extraPlace.processPlaces).ToArray();   
            for (int i = 0; i < extraPlace.processPlaces.Length; i++)
            {
                freePlaces.Enqueue(extraPlace.processPlaces[i]);
            }
        }
        if (extraPlace.extraQueuePlaces.Length > 0)
        {
            queuePlaces = queuePlaces.Concat(extraPlace.extraQueuePlaces).ToArray();   
        }
    }

    protected virtual void OnUpdate()
    {
        if (!wasBought && IsBought)
        {
            wasBought = true;
            OnPurchase();
        }
    }

    protected virtual void OnPurchase()
    {
        FillQueueWithCitizens();
    }

    protected virtual void FillQueueWithCitizens()
    {
        foreach (var queuePlace in queuePlaces)
        {
            CitizenSpawner.Instance.SpawnCitizenFor(queuePlace, this);
        }
    }

    protected virtual void UpdateProcessed(CitizenController citizen)
    {
        if (CanAddProgress(citizen))
        {
            citizen.WishesController.AddProgress(this, Time.deltaTime / FullProgressTime);
        }
    }

    protected abstract void OnLeave(CitizenController citizen);

    public bool HasAnyone()
    {
        return processed.Count + queue.Count + approaching.Count == 0;
    }
    
    public virtual bool IsWorking()
    {
        return upgradable == null || upgradable.IsBought;
    }

    public bool HasInQueueOrProcessed(CitizenController citizen)
    {
        return approaching.Contains(citizen) || queue.Contains(citizen) || processed.Contains(citizen) || leaving.Contains(citizen);
    }

    protected virtual bool CanAddProgress(CitizenController citizen)
    {
        if (citizenPlacesDict.ContainsKey(citizen))
        {
            return citizenPlacesDict[citizen].CanAddProgress(citizen) && (wishPostProcessor == null || 
                                                                          wishPostProcessor.HasMorePlace());
        }
        else
        {
            Debug.LogError($"who is checking this bs");
        }
        return false;
    }
    
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
        if (exit == null)
        {
            return citizen.transform.position;
        }
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
        if (!processed.Contains(citizen))
        {
            processed.Add(citizen);   
            OnAddToProcessed(citizen);
        }
        var place = GetProcessPlaceFor(citizen);
        var rot = GetProcessRotFor(citizen);
        citizen.transform.DORotateQuaternion(rot, 0.3f);
        citizen.transform.DOMove(place, 0.3f);
    }

    protected virtual void OnAddToProcessed(CitizenController citizen)
    {
        
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
        citizen.WishesController.RemoveWish(this);
        #if UNITY_EDITOR
        if (approaching.Contains(citizen) || queue.Contains(citizen))
        {
            Debug.LogError($"something wrong with this granter bro");
        }
        #endif
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
        var rwrd = Reward;
        if (upgradable != null && upgradable.Level > 1)
        {
            rwrd = GetRewardWithMultipliers();
        }
        if (currencyStack != null)
        {
            currencyStack.MoveCurrencyToMe(rwrd, citizen.transform.position + Vector3.up);   
        }
        else if(Reward > 0)
        {
            CurrencyStackBehaviour.SpawnSingleCurrency(Reward, citizen.transform.position);
        }
    }

    public int GetRewardWithMultipliers()
    {
        return Mathf.RoundToInt(Reward * _gameConfig.incomeIncrease * IncomeMultiplier);
    }

    public virtual bool CanAddOneMore()
    {
        return approaching.Count + queue.Count + processed.Count < queuePlaces.Length + processPlaces.Length && 
              (coolDownTimer == null || coolDownTimer.CanAddMore());
    }

    private ProcessPlace DequeuePlace()
    {
        var place = freePlaces.Dequeue();
        return place;
    }
    
    private void EnqueuePlace(ProcessPlace place)
    {
        freePlaces.Enqueue(place);
    }

    public bool IsProcessed(CitizenController target)
    {
        return processed.Contains(target);
    }

    public int GetCitizensCount()
    {
        return processPlaces.Length + queuePlaces.Length;
    }
    
    protected void AddMoney(int count, Vector3 from)
    {
        currencyStack.MoveCurrencyToMe(count, from);
    }

    public bool CanProcess()
    {
        return wishPostProcessor == null || wishPostProcessor.HasMorePlace();
    }

    public Vector3 GetMiddleProcessPlace()
    {
        var middle = Vector3.zero;
        foreach (var place in processPlaces)
        {
            middle += place.Position;
        }

        return middle / processPlaces.Length;
    }

    public void SubscribePlayerProcess(Action<Transform> onProcess)
    {
        this.onPlayerProcess = onProcess;
    }

    public Transform GetStackPlace()
    {
        if (currencyStack == null)
        {
            return transform;
        }
        return currencyStack.transform;
    }
}