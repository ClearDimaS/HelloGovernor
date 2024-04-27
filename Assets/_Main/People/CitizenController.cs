using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class CitizenController : MonoBehaviour
{
    [Inject] private EnvironmentManager environment;
    
    [SerializeField] private Walker walker;

    public Walker Walker => walker;
    
    private void Update()
    {
        if (!walker.IsMoving)
        {
            var pos = GetRandomPos();
            walker.SetWalkTarget(pos);
        }
    }
    
    public void PlaceRandom()
    {
        var pos = GetRandomPos();
        walker.Place(pos);
    }
        
    private Vector3 GetRandomPos()
    {
        var pos = environment.mapCenter + new Vector3(
            Random.Range(-environment.mapSize.x / 2f, environment.mapSize.x / 2f),
            0,
            Random.Range(-environment.mapSize.z / 2f, environment.mapSize.z / 2f));
        return pos;
    }

    public void Place(Vector3 citizenPlacePosition)
    {
        walker.Place(citizenPlacePosition);
    }
}
