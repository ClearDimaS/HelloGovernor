using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class LevelState : GameStateBase
{
    [Inject] private PlayerDataRepository playerDataRepository;

    public override void EnterState()
    {
        UI_Manager.Instance.OpenScreen(EScreenType.Game);
    }

    [Button]
    private void SetLevelIndex(int newValue)
    {
        var data = playerDataRepository.GetData();
        data.levelIndex = newValue;
        playerDataRepository.SetData(data);
    }
    
    public override void ExitState()
    {
        UI_Manager.Instance.CloseScreen(EScreenType.Game);
    }
}
