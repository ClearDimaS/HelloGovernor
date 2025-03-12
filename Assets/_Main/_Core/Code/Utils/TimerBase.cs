using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBase : MonoBehaviour
{
    [SerializeField] private SpriteRadialFiller radialFiller;
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    private float lastFill = -1f;
    public void SetIcon(Sprite icon)
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sprite = icon;   
        }
    }

    public void SetProgress(float progress)
    {
        if (lastFill != progress)
        {
            lastFill = progress;
            radialFiller.fillAmount = lastFill;
        }
    }

    public void SetColor(Color color)
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = color;
        }
    }
}
