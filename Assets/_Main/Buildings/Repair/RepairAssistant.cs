using System;
using UnityEngine;
using Zenject;

public interface IRepairable
{
    public Transform Place { get; }
    public bool CanRepair(IRepairer repairAssistant);
}
public interface IRepairer
{

}

public class RepairAssistant : MonoBehaviour, IRepairer
{
    [Inject] protected HousesManager housesManager;

    [SerializeField] protected Walker walker;
    
    protected HouseBuilding buildingTarget;

    private void Update()
    {
        if (buildingTarget == null || !buildingTarget.CanRepair(this))
        {
            RefreshTarget();
        }
        else
        {
            buildingTarget.CanRepair(this, out IRepairable target);
            walker.MoveToTarget(target.Place.position, null);  
        }
    }

    private void RefreshTarget()
    {
        buildingTarget = housesManager.GetBrokenBuildingForRepair();
        if (buildingTarget != null)
        {
            buildingTarget.CanRepair(this, out IRepairable target);
            walker.MoveToTarget(target.Place.position, null);   
        }
    }
}