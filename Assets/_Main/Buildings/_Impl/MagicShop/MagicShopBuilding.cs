using UnityEngine;

public class MagicShopBuilding : BuildingBase
{
    [SerializeField] private Transform policePlace;

    public Transform PolicePlace => policePlace;
}