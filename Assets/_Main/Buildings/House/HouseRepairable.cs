using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HouseRepairable : MonoBehaviour, IRepairable
{
    [Inject] private GameConfig gameConfig;

    [SerializeField] private GameObject[] grxVariants;
    [SerializeField] private ScaleAnimator needRepairContent;
    
    protected IRepairer repairer;

    protected float repairProgress = 0f;
    public bool IsBroken { get; private set; }
    
    public Transform Place => transform;
    
    private void Awake()
    {
        needRepairContent.Hide(true);
    }

    private void OnEnable()
    {
        var rand = Random.Range(0, grxVariants.Length);
        for (int i = 0; i < grxVariants.Length; i++)
        {
            grxVariants[i].SetActive(i == rand);
        }
    }

    private void Update()
    {
        if (IsBroken && repairer != null)
        {
            repairProgress += Time.deltaTime / gameConfig.repairHouseTime;
            if (repairProgress >= 1f)
            {
                Repair();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null || other.isTrigger)
        {
            return;
        }

        if (!other.attachedRigidbody.TryGetComponent(out IRepairer repairer))
        {
            return;
        }

        if (this.repairer != null)
        {
            return;
        }
        this.repairer = repairer;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null || other.isTrigger)
        {
            return;
        }

        if (!other.attachedRigidbody.TryGetComponent(out IRepairer repairer))
        {
            return;
        }

        if (this.repairer == repairer)
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
        IsBroken = true;
        needRepairContent.Show(false);
    }

    private void Repair()
    {
        repairProgress = 0f;
        IsBroken = false;
        needRepairContent.Hide(false);
    }
}