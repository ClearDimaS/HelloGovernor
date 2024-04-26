using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HouseBuilding : MonoBehaviour
{
    [Inject] private HousesManager housesManager;
    [field: SerializeField] public Transform CitizenPlace { get; private set; }
    
    public int CitizensCount => 2;

    private void Awake()
    {
        housesManager.Add(this);
    }

    private void Start()
    {
        RefreshState();
    }

    private void RefreshState()
    {

    }
}
