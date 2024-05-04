using UnityEngine;

public class BankBuilding : BuildingBase
{
    [SerializeField] protected Transform assistantPlace;
    
    public Transform AssistantPlace => assistantPlace;
}