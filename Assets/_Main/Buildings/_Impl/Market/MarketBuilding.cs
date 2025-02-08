using UnityEngine;

public class MarketBuilding : BuildingBase
{
    [SerializeField] private Transform policePlace;

    public Transform PolicePlace => policePlace;
}