using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HouseRepairable : SimpleRepairerPhysicsBehaviour, IRepairable
{
    [Inject] private SoundManager soundManager;
    [Inject] private PlayerController player;
    [Inject] private WishesCollectionConfig gameConfig;

    [SerializeField] private int levelIndex;
    [SerializeField] private TimerBase timer;
    [SerializeField] private GameObject[] gfxVariants;
    [SerializeField] private ScaleAnimator needRepairContent;
    [SerializeField] private ScaleAnimator levelContent;
    [SerializeField] private ParticleSystem[] donePSs;
    
    protected IRepairer repairer;

    protected float breakTimer;
    protected float lastBreakTime;
    protected HouseBuilding houseBuilding;
    protected bool isAdded;
    protected float repairProgress = 0f;
    public bool IsBroken { get; private set; }
    
    public Transform Place => transform;

    protected override void OnAwake()
    {
        base.OnAwake();
        needRepairContent.Hide(true);
        houseBuilding = GetComponentInParent<HouseBuilding>();
    }

    private void OnEnable()
    {
        var rand = Random.Range(0, gfxVariants.Length);
        for (int i = 0; i < gfxVariants.Length; i++)
        {
            gfxVariants[i].SetActive(i == rand);
            donePSs[i].gameObject.SetActive(i == rand);
        }
    }

    private void Start()
    {
        needRepairContent.Hide(true);
        breakTimer = Random.Range(gameConfig.breakTimerMinMax.x, gameConfig.breakTimerMinMax.y);
        if (houseBuilding.Level > levelIndex)
        {
            levelContent.Show(true);
        }
        else
        {
            levelContent.Hide(true);   
        }
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (!isAdded && houseBuilding.Level > levelIndex)
        {
            levelContent.Show(false);
            houseBuilding.AddRepairable(this);
            isAdded = true;
            lastBreakTime = Time.time;
            breakTimer = Random.Range(gameConfig.breakTimerMinMax.x, gameConfig.breakTimerMinMax.y);
        }

        if (isAdded)
        {
            if (IsBroken)
            {
                timer.SetProgress(repairProgress);
                if (repairer != null)
                {
                    repairProgress += Time.deltaTime / gameConfig.repairHouseTime;
                    if (repairProgress >= 1f)
                    {
                        Repair();
                    }
                }     
            }
            else if (Time.time - lastBreakTime > breakTimer)
            {
                Break();
            }
        }
    }

    protected override void OnEnter(IRepairer component)
    {
        base.OnEnter(component);
        if (this.repairer != null)
        {
            return;
        }
        this.repairer = component;
    }

    protected override void OnLeave(IRepairer component)
    {
        base.OnLeave(component);
        if (this.repairer == component)
        {
            this.repairer = null;
        }
    }

    public bool CanRepair(IRepairer repairAssistant)
    {
        if (!IsBroken)
        {
            return false;
        }
        if (repairer == null)
        {
            return true;
        }

        return this.repairer == repairer;
    }
    
    public void Break()
    {
        lastBreakTime = Time.time;
        breakTimer = Random.Range(gameConfig.breakTimerMinMax.x, gameConfig.breakTimerMinMax.y);
        repairProgress = 0f;
        timer.SetProgress(0f);
        IsBroken = true;
        needRepairContent.Show(false);
    }

    private void Repair()
    {
        repairProgress = 0f;
        timer.SetProgress(0f);
        IsBroken = false;
        needRepairContent.Hide(false);
        foreach (var ps in donePSs)
        {
            if (ps.gameObject.activeInHierarchy)
            {
                ps.Play();       
            }
        }

        if (repairer == player)
        {
            soundManager.PlayerRepair();
        }

        CurrencyStackBehaviour.SpawnSingleCurrency(gameConfig.repairHouseReward, transform.position);
    }
}