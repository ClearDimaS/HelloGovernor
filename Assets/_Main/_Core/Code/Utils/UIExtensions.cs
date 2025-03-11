using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class UIExtensions
{
    private static Dictionary<RectTransform, int> rects = new (); 
    
    public static void UpdateState(this Button button, bool isShown, bool immediate = false)
    {
        UpdateState(button.targetGraphic.rectTransform, isShown, immediate);
    }
    
    public static void UpdateState(this Image img, bool isShown, bool immediate = false)
    {
        UpdateState(img.rectTransform, isShown, immediate);
    }
    
    public static void UpdateState(this RectTransform rect, bool isShown, bool immediate = false)
    {
        if (!rects.ContainsKey(rect))
        {
            rects[rect] = -99;
        }
        
        if (immediate)
        {
            if (isShown)
            {
                if (rects[rect] != 2)
                {
                    rects[rect] = 2;
                    SetState(rect, 1f);
                }
            }
            else
            {
                if (rects[rect] != 1)
                {
                    rects[rect] = 1;
                    SetState(rect, 0f);
                }
            }   
        }
        else
        {
            if (isShown)
            {
                if (rects[rect] != -1)
                {
                    rects[rect] = -1;
                    rect.DOKill();
                    rect.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.InOutCubic).OnComplete(() =>
                    {
                        rect.DOScale(Vector3.one * 1f, 0.1f).SetEase(Ease.InOutCubic);
                    });
                }
            }
            else
            {
                if (rects[rect] != -2)
                {
                    rects[rect] = -2;
                    rect.DOKill();
                    rect.DOScale(Vector3.one * 0f, 0.3f).SetEase(Ease.InOutCubic);
                }
            }
        }
    }

    public static void SetState(RectTransform rect, float t)
    {
        rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
        
        if (Math.Abs(t - 0f) < 0.01f)
        {
            rect.gameObject.SetActive(false);
        }
        if (Math.Abs(t - 1f) < 0.01f)
        {
            rect.gameObject.SetActive(true);
        }
    }
}
