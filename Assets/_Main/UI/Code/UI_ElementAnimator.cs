using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UI_ElementAnimator : MonoBehaviour
{
    [SerializeField] private float duration = 0f;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private AnimationCurve alphaCurve;

    public bool IsShown => state == 1;
    private int state = -1;
    private CanvasGroup group;
    private float currentT;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Animate(Action doneHandler = null, bool instant = false)
    {
        transform.DOKill();
        var t = currentT;
        state = 1;
        if (instant)
        {
            currentT = 1f;
            ApplyT(currentT);
            doneHandler?.Invoke();
            return;
        }
        
        DOTween.To(() => t, x => t = x, 1f, duration * (1f - currentT)).OnUpdate(() =>
        {
            currentT = t;
            ApplyT(currentT);
        }).OnComplete(() =>
        {
            currentT = 1f;
            ApplyT(currentT);
            doneHandler?.Invoke();
        }).SetTarget(transform).SetEase(Ease.Linear);
    }

    public void AnimateBack(Action doneHandler = null, bool instant = false)
    {
        transform.DOKill();
        var t = currentT;
        state = 0;
        if (instant)
        {
            currentT = 0f;
            ApplyT(currentT);
            doneHandler?.Invoke();
            return;
        }
        
        DOTween.To(() => t, x => t = x, 0f, duration * currentT).OnUpdate(() =>
        {
            currentT = t;
            ApplyT(currentT);
        }).OnComplete(() =>
        {
            currentT = 0f;
            ApplyT(currentT);
            doneHandler?.Invoke();
        }).SetTarget(transform).SetEase(Ease.Linear);
    }

    private void ApplyT(float t)
    {
        transform.localScale = Vector3.one * scaleCurve.Evaluate(t);
        group.alpha = alphaCurve.Evaluate(t);
    }
}
