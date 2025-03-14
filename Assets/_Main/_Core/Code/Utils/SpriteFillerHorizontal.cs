using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpriteFillerHorizontal : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;

    public float fillAmount
    {
        get
        {
            if (!originalWidth.HasValue)
            {
                originalWidth = spriteRenderer.size.x;
            }
            return spriteRenderer.size.x/originalWidth.Value;
        }
        set
        {
            if (!originalWidth.HasValue)
            {
                originalWidth = spriteRenderer.size.x;
            }
            
            var fill = Mathf.Clamp01(value);
            var size = spriteRenderer.size;
            size.x = fill * originalWidth.Value;
            spriteRenderer.size = size;
        }
    }

    protected float? originalWidth;

    public void Fill(float fill, float timer)
    {
        var t = 0f;
        var start = fillAmount;
        DOTween.To(() => t, x => t = x, 1f, timer).OnUpdate(() =>
        {
            fillAmount = Mathf.Lerp(start, fill, t);
        }).OnComplete(() =>
        {
            fillAmount = fill;
        });
    }
}
