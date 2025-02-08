using UnityEngine;

public class UmbrellaBuilding : BuildingBase
{
    [SerializeField] private Transform policePlace;

    public Transform PolicePlace => policePlace;
}