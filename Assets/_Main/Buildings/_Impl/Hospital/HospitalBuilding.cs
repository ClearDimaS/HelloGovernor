using UnityEngine;

public class HospitalBuilding : BuildingBase
{
    [SerializeField] private Transform policePlace;

    public Transform PolicePlace => policePlace;
}