using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CapacityIndicator : MonoBehaviour
{
    [SerializeField] private TextMesh[] currentTexts;
    [SerializeField] private TextMesh[] maxTexts;
    [SerializeField] private Transform content;
    [SerializeField] private SpriteAlphaGroup alphaGroup;
    [SerializeField] protected float showAfterChangeTime = 2f;
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private SpriteRadialFiller[] fillers;
    
    protected float lastChangeTime = -10;
    [SerializeField] protected int lastCount;
    protected Interactor interactor;

    private void Awake()
    {
        interactor = GetComponentInParent<Interactor>();
        alphaGroup.Fade(0f, 0f);
        foreach (var filler in fillers)
        {
            filler.fillAmount = 0f;   
        }
    }

    private void Update()
    {
        var show = Time.time - lastChangeTime < showAfterChangeTime;
        if (lastCount != interactor.interactables.Count)
        {
            lastCount = interactor.interactables.Count;
            lastChangeTime = Time.time;
            var curString = lastCount.ToString();
            foreach (var currentText in currentTexts)
            {
                currentText.text = curString;
            }
            if (lastCount > 0)
            {
                var maxString = interactor.GetCurrentMaxPlaces().ToString();
                foreach (var maxText in maxTexts)
                {
                    maxText.text = maxString;
                }
            }

            var endVal = lastCount / (float)interactor.GetCurrentMaxPlaces();
            var startVal = fillers[0].fillAmount;
            var t = 0f;
            DOTween.To(() => t, x => t = x, 1f, 0.3f).OnUpdate(() =>
            {
                foreach (var filler in fillers)
                {
                    filler.fillAmount = Mathf.Lerp(startVal, endVal, t); 
                }
            }).OnComplete(() =>
            {
                foreach (var filler in fillers)
                {
                    filler.fillAmount = endVal;
                }
            });
        }

        if (alphaGroup.IsShown != show)
        {
            alphaGroup.Fade(show ? 1f : 0f, fadeTime);
        }
    }
}
