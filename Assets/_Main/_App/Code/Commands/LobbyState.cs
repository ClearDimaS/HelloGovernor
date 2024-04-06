using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyState : GameStateBase
{
    private LevelState levelState;
    private void Awake()
    {
        levelState = FindObjectOfType<LevelState>(true);
    }

    public override void EnterState()
    {
        UI_Manager.Instance.OpenScreen(EScreenType.Lobby);
    }

    public override void ExitState()
    {
        UI_Manager.Instance.CloseScreen(EScreenType.Lobby);
    }
}
