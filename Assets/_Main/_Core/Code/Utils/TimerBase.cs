using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBase : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image fillImage;

    private float lastFill = -1f;
    public void SetIcon(Sprite icon)
    {
        iconImage.sprite = icon;
    }

    public void SetProgress(float progress)
    {
        if (lastFill != progress)
        {
            lastFill = progress;
            fillImage.fillAmount = lastFill;
        }
    }
}
