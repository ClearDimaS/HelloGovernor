using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SpriteAlphaGroup : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer[] sprites;
    [SerializeField] protected TextMesh[] texts;

    protected float[] spriteStartAlphas;
    public bool IsShown { get; protected set; }
    public float Alpha { get; protected set; }

    private void Awake()
    {
        spriteStartAlphas = sprites.Select(x => x.color.a).ToArray();
    }

    private void Start()
    {
        if (!IsShown)
        {
            ApplyAlpha(0f);
        }
        else
        {
            ApplyAlpha(1f);
        }
    }

    public void Fade(float target, float time)
    {
        if (target < 0.01f)
        {
            IsShown = false;
        }
        else
        {
            IsShown = true;
        }
        var start = Alpha;
        var t = 0f;
        DOTween.To(() => t, x => t = x, 1f, time).OnUpdate(() =>
        {
            var a = Mathf.Lerp(start, target, t);
            ApplyAlpha(a);
            Alpha = a;
        }).OnComplete(() =>
        {
            ApplyAlpha(target);
            Alpha = target;
        }).SetTarget(this);
    }

    protected void ApplyAlpha(float value)
    {
        for (var i = 0; i < sprites.Length; i++)
        {
            var newAlpha = Mathf.Lerp(0, spriteStartAlphas[i], value);
            var sprite = sprites[i];
            var col = sprite.color;
            col.a = newAlpha;
            sprite.color = col;
        }

        foreach (var text in texts)
        {
            var col = text.color;
            col.a = value;
            text.color = col;
        }
    }
}