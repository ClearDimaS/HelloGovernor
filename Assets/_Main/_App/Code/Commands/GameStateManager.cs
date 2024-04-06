using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    private GameStateBase currentState; 
    private Dictionary<Type, GameStateBase> states = new ();

    protected override void OnCreated()
    {
        base.OnCreated();
        states = GetComponentsInChildren<GameStateBase>(true).ToDictionary(x => x.GetType(), x => x);
        foreach (var state in states)
        {
            state.Value.gameObject.SetActive(false);   
        }
    }

    public void SetState<T>() where T : GameStateBase
    {
        if (currentState != null)
        {
            currentState.ExitState();
            currentState.gameObject.SetActive(false);
        }
        
        var state = states[typeof(T)];
        currentState = state;
        currentState.gameObject.SetActive(true);
        currentState.EnterState();
    }
}
