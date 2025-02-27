using DG.Tweening;
using UnityEngine;

public class SpriteAlphaGroup : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer[] sprites;
    [SerializeField] protected TextMesh[] texts;

    public bool IsShown { get; protected set; }
    public float Alpha { get; protected set; }

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
        this.DOKill();
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
        foreach (var sprite in sprites)
        {
            var col = sprite.color;
            col.a = value;
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