using System;
using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class GameManager : Singleton<GameManager>
{
    [Inject] private PlayerDataRepository repository;
    
    protected override void OnCreated()
    {
        base.OnCreated();
    }

    private void Start()
    {
        LaunchGame();
    }

    public void LaunchLobby()
    {
        GameStateManager.Instance.SetState<LobbyState>();
    }

    public void LaunchGame()
    {
        GameStateManager.Instance.SetState<LevelState>();
    }

    [Button]
    private void LoadLevel()
    {
        var playerData = repository.GetData();
        repository.SetData(playerData);
        LaunchLobby();
    }
}
