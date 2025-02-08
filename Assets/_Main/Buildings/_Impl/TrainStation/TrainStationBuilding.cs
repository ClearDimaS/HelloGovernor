using UnityEngine;

public class TrainStationBuilding : BuildingBase
{
    [SerializeField] private Transform policePlace;

    public Transform PolicePlace => policePlace;
}