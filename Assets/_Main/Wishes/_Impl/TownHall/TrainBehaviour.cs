using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ETrainState
{
    Away,
    Arrival,
    Departure
}
public class TrainBehaviour : Singleton<TrainBehaviour>
{
    [SerializeField] private Transform trainRoot;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private ETrainState startState;
    [Header("Places")]
    [SerializeField] private Transform trainPlaceArrival;
    [SerializeField] private Transform trainPlaceDeparture;
    [SerializeField] private Transform trainPlaceAway;
    [Header("timings")]
    [SerializeField] private float departureTime = 10f;
    [SerializeField] private float arrivalTime = 10f;
    [SerializeField] private float awayMinTime = 10f;

    [SerializeField] private Transform[] entries;
    
    private float inStateTime = 0f;
    private ETrainState currentState;
    
    protected override void OnCreated()
    {
        base.OnCreated();
        MoveToState(startState, true);
    }

    private void Update()
    {
        inStateTime+=Time.deltaTime;
        var maxTime = GetDuration(currentState);
        if (inStateTime >= maxTime)
        {
            MoveToState(GetNextState(currentState));
        }
    }

    public Transform GetNearestEntry(Transform fromPlace)
    {
        var entryPlace = entries[0];
        var distMin = Mathf.Infinity;
        foreach (var entry in entries)
        {
            var sqrDist = (entry.position - fromPlace.position).sqrMagnitude;
            if (sqrDist < distMin)
            {
                distMin = sqrDist;
                entryPlace = entry;
            }
        }

        return entryPlace;
    }
    
    public Transform GetRandomEntry()
    {
        return entries[Random.Range(0, entries.Length - 1)];
    }

    public ETrainState GetState()
    {
        return currentState;
    }

    public float GetTimeLeft()
    {
        return GetDuration(currentState) - inStateTime;
    }
    
    private void MoveToState(ETrainState newState, bool immediate = false)
    {
        currentState = newState;
        var place = GetPlace(currentState);
        var moveTime = (trainRoot.position - place.position).magnitude/moveSpeed;
        if (immediate)
        {
            moveTime = 0f;
        }
        trainRoot.DOMove(place.position, moveTime);
        inStateTime = 0f;
    }

    private Transform GetPlace(ETrainState state)
    {
        return state switch
        {
            ETrainState.Arrival => trainPlaceArrival,
            ETrainState.Departure => trainPlaceDeparture,
            ETrainState.Away => trainPlaceAway,
            _ => throw new NotImplementedException()
        };
    }
    private float GetDuration(ETrainState state)
    {
        return state switch
        {
            ETrainState.Arrival => arrivalTime,
            ETrainState.Departure => departureTime,
            ETrainState.Away => awayMinTime,
            _ => throw new NotImplementedException()
        };
    }
    
    private ETrainState GetNextState(ETrainState state)
    {
        return state switch
        {
            ETrainState.Arrival => ETrainState.Departure,
            ETrainState.Departure => ETrainState.Away,
            ETrainState.Away => ETrainState.Arrival,
            _ => throw new NotImplementedException()
        };
    }
}
