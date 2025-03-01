using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class ButtonClickPlayer : MonoBehaviour
{
    [Inject] private SoundManager audioManager;
    [Inject] private VibrationManager vibrationManager;

    private void Awake()
    {
        var button = GetComponent<EventfullButton>();
        button.OnPointerUpCommand.Subscribe(_ => PlayEffects()).AddTo(gameObject);
    }

    private void PlayEffects()
    {
        audioManager.Click(GetInstanceID());
        vibrationManager.Click();
    }
}
