using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapacityIndicator : MonoBehaviour
{
    [SerializeField] private TextMesh currentText;
    [SerializeField] private TextMesh maxText;
    [SerializeField] private Transform content;
    [SerializeField] private SpriteAlphaGroup alphaGroup;
    [SerializeField] protected float showAfterChangeTime = 2f;
    [SerializeField] private float fadeTime = 1f;
    
    protected float lastChangeTime;
    protected int lastCount;
    protected Interactor interactor;

    private void Awake()
    {
        interactor = GetComponentInParent<Interactor>();
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
        }

        if (alphaGroup.IsShown != show)
        {
            alphaGroup.Fade(show ? 1f : 0f, fadeTime);
        }
    }
}
