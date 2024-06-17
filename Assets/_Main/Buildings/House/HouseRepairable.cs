using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HouseRepairable : SimpleRepairerPhysicsBehaviour, IRepairable
{
    [Inject] private PlayerController player;
    [Inject] private CurrencySingleStackPool currencyPool;
    [Inject] private CompassManager compassManager;
    [Inject] private GameConfig gameConfig;

    [SerializeField] private TimerBase timer;
    [SerializeField] private GameObject[] grxVariants;
    [SerializeField] private ScaleAnimator needRepairContent;
    [SerializeField] private ParticleSystem[] donePSs;
    
    protected IRepairer repairer;

    protected float repairProgress = 0f;
    public bool IsBroken { get; private set; }
    
    public Transform Place => transform;

    protected override void OnAwake()
    {
        base.OnAwake();
        needRepairContent.Hide(true);
    }

    private void OnEnable()
    {
        var rand = Random.Range(0, grxVariants.Length);
        for (int i = 0; i < grxVariants.Length; i++)
        {
            grxVariants[i].SetActive(i == rand);
            donePSs[i].gameObject.SetActive(i == rand);
        }
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        timer.SetProgress(repairProgress);
        if (IsBroken && repairer != null)
        {
            repairProgress += Time.deltaTime / gameConfig.repairHouseTime;
            if (repairProgress >= 1f)
            {
                Repair();
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
        repairProgress = 0f;
        timer.SetProgress(0f);
        IsBroken = true;
        needRepairContent.Show(false);
        compassManager.AddTarget(transform, ECompasTarget.Repair);
    }

    private void Repair()
    {
        repairProgress = 0f;
        timer.SetProgress(0f);
        IsBroken = false;
        needRepairContent.Hide(false);
        compassManager.RemoveTarget(transform, ECompasTarget.Repair);
        foreach (var ps in donePSs)
        {
            if (ps.gameObject.activeInHierarchy)
            {
                ps.Play();       
            }
        }

        if (repairer == player)
        {
            SoundManager.Instance.PlayerRepair();
        }

        var reward = gameConfig.repairHouseReward;
        var currency = currencyPool.GetElement();
        currency.transform.position = transform.position + Vector3.up + new Vector3(1f, 0, 1f).AxisToRandomDir();
        currency.Initialize(reward);
        currency.AddForce(Vector3.up * 3 + new Vector3(1f, 0, 1f).AxisToRandomDir() * 2);
    }
}