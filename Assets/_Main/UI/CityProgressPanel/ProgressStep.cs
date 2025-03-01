using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityProgressStep : MonoBehaviour
{
    [SerializeField] private TMP_Text stageNumberText;
    [SerializeField] private Image icon;
    [SerializeField] private UI_ElementAnimator completeAnimator;
    [SerializeField] private float completionTimer = 2f;

    private void Awake()
    {
        completeAnimator.AnimateBack(instant:true);
    }

    public void Init(Sprite sprite, int stageNumber)
    {
        icon.sprite = sprite;
        stageNumberText.text = stageNumber.ToString();
    }

    public void MarkCompleted(Action action)
    {
        completeAnimator.Animate();
        UniTask.Delay(TimeSpan.FromSeconds(completionTimer)).ContinueWith(() =>
        {
            completeAnimator.AnimateBack();
            action?.Invoke();
        });
    }
}
