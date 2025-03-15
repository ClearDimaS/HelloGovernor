using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public interface IItemsCountable
{
    public int GetCurrentPlaces();
    public int GetMaxPlaces();
}

public class CapacityIndicator : CulledBehaviour
{
    [SerializeField] private TextMesh[] currentTexts;
    [SerializeField] private TextMesh[] maxTexts;
    [SerializeField] private Transform content;
    [SerializeField] private SpriteAlphaGroup alphaGroup;
    [SerializeField] protected float showAfterChangeTime = 2f;
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private SpriteRadialFiller[] fillers;
    
    protected Dictionary<int, string> stringsDict = new ();
    protected float lastChangeTime = -10;
    [SerializeField] protected int lastCount;
    protected IItemsCountable interactor;

    protected override void OnAwake()
    {
        base.OnAwake();
        interactor = GetComponentInParent<IItemsCountable>();
        alphaGroup.Fade(0f, 0f);
        foreach (var filler in fillers)
        {
            filler.fillAmount = 0f;   
        }
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        var show = Time.time - lastChangeTime < showAfterChangeTime;
        if (lastCount != interactor.GetCurrentPlaces())
        {
            lastCount = interactor.GetCurrentPlaces();
            lastChangeTime = Time.time;
            var curString = GetString(lastCount);
            foreach (var currentText in currentTexts)
            {
                currentText.text = curString;
            }
            if (lastCount > 0)
            {
                var maxString = GetString(interactor.GetMaxPlaces());
                foreach (var maxText in maxTexts)
                {
                    maxText.text = maxString;
                }
            }

            var endVal = lastCount / (float)interactor.GetMaxPlaces();
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

    private string GetString(int count)
    {
        if (!stringsDict.ContainsKey(count))
        {
            stringsDict[count] = count.ToString();
        }
        return stringsDict[count];
    }

}
