using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CapacityIndicator : MonoBehaviour
{
    [SerializeField] private TextMesh currentText;
    [SerializeField] private TextMesh maxText;
    [SerializeField] private Transform content;
    [SerializeField] private SpriteAlphaGroup alphaGroup;
    [SerializeField] protected float showAfterChangeTime = 2f;
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private SpriteRadialFiller filler;
    
    protected float lastChangeTime = -10;
    [SerializeField] protected int lastCount;
    protected Interactor interactor;

    private void Awake()
    {
        interactor = GetComponentInParent<Interactor>();
        alphaGroup.Fade(0f, 0f);
        filler.fillAmount = 0f;
    }

    private void Update()
    {
        var show = Time.time - lastChangeTime < showAfterChangeTime;
        if (lastCount != interactor.interactables.Count)
        {
            lastCount = interactor.interactables.Count;
            lastChangeTime = Time.time;
            currentText.text = lastCount.ToString();
            if (lastCount > 0)
            {
                maxText.text = interactor.GetCurrentMaxPlaces().ToString();
            }

            var endVal = lastCount / (float)interactor.GetCurrentMaxPlaces();
            var startVal = filler.fillAmount;
            var t = 0f;
            DOTween.To(() => t, x => t = x, 1f, 0.3f).OnUpdate(() =>
            {
                filler.fillAmount = Mathf.Lerp(startVal, endVal, t);
            }).OnComplete(() =>
            {
                filler.fillAmount = endVal;
            });
        }

        if (alphaGroup.IsShown != show)
        {
            alphaGroup.Fade(show ? 1f : 0f, fadeTime);
        }
    }
}
