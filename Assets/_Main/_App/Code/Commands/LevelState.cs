using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class LevelState : GameStateBase
{
    public override void EnterState()
    {
        UI_Manager.Instance.OpenScreen(EScreenType.Game);
    }

    public override void ExitState()
    {
        UI_Manager.Instance.CloseScreen(EScreenType.Game);
    }
}
