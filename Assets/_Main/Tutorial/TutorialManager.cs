using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TutorialManager : Singleton<TutorialManager>
{
    [Inject] private PlayerDataRepository playerRepository;
    private bool isShown;

    public void TryRunTutorial()
    {
        StopAllCoroutines();
        var playerData = playerRepository.GetData();
    }
}
