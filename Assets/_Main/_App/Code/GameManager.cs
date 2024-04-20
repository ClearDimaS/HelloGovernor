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
    
    [SerializeField] private int loadLevel;
    
    private LevelState levelState;

    public event Action<int> levelStartEvent;
    public event Action<int> levelFailEvent;
    public event Action<int> levelSuccessEvent;
    
    protected override void OnCreated()
    {
        base.OnCreated();
        levelState = FindObjectOfType<LevelState>(true);
    }

    private void Start()
    {
        LaunchGame();
    }

    public void LaunchLobby()
    {
        UnityEngine.Random.InitState(repository.GetData().levelIndex);
        GameStateManager.Instance.SetState<LobbyState>();
    }

    public void LaunchGame()
    {
        GameStateManager.Instance.SetState<LevelState>();
        levelStartEvent?.Invoke(repository.GetData().LevelNumber);
    }
    
    public void ReLaunchGame()
    {
        LaunchLobby();
    }
    
    public void RestartGame()
    {
        levelFailEvent?.Invoke(repository.GetData().LevelNumber);
        LaunchLobby();
        LaunchGame();
    }

    [Button]
    private void LoadLevel()
    {
        var playerData = repository.GetData();
        playerData.levelIndex = loadLevel;
        repository.SetData(playerData);
        LaunchLobby();
    }
}
