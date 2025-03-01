using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

public class RewardUIElement : MonoBehaviour, IResetable
{
    private RewardUIElementsPool pool;

    [SerializeField] protected RectTransform rectTransform;
    [SerializeField] protected Image icon;
    [SerializeField] protected float toWalletDelay = 0.3f;
    [SerializeField] protected float bounceHeight = 1f;
    [SerializeField] protected float bounceTime;
    [SerializeField] protected float toWalletTime;
    [SerializeField] protected Ease bounceEase;
    [SerializeField] protected Ease toWalletEase;
    
    [SerializeField] protected float scaleInTime;
    [SerializeField] protected Ease scaleInEase;
    
    [SerializeField] protected float scaleOutDelay;
    [SerializeField] protected float scaleOutTime;
    [SerializeField] protected Ease scaleOutEase;
    [SerializeField] protected Vector2 viewPortRadiusMinMax = new Vector2(0.1f, 0.2f);
    private RectTransform parent;
    private Action onComplete;
    
    public float MoveTime => bounceTime + toWalletTime;
    
    public void Init(float durationMult, Vector2 startViewPort, Vector2 endViewport, Action onComplete, RewardUIElementsPool pool)
    {
        this.pool = pool;
        if (parent == null || parent != transform.parent)
        {
            parent = transform.parent.GetComponent<RectTransform>();
        }
        this.onComplete = onComplete;
        icon.color = Color.white;
        
        var endPos = endViewport;
        
        var t = 0f;
        var randomBounceDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        transform.DOScale(Vector3.one, scaleInTime).SetEase(scaleInEase);
        var range = Random.Range(viewPortRadiusMinMax.x, viewPortRadiusMinMax.y);
        var startPos = startViewPort;
        var middlePos = startViewPort + randomBounceDir * range;
        
        SetViewportPosition(startPos);
        DOTween.To(() => t, x => t = x, 1f, bounceTime * durationMult).OnUpdate(() =>
        {
            var newPos = Vector2.Lerp(startPos, middlePos, t);
            if (t < 0.5f)
            {
                newPos += Vector2.up * Mathf.Sqrt(Mathf.PingPong(t, 0.5f)/0.5f) * (bounceHeight);
            }
            else
            {
                newPos += Vector2.up * Mathf.Pow(Mathf.PingPong(t, 0.5f)/0.5f, 2) * (bounceHeight);
            }

            SetViewportPosition(newPos);
        }).SetEase(bounceEase).OnComplete(() =>
        {
            t = 0f;
            transform.DOScale(Vector3.one * 0.15f, scaleOutTime).SetEase(scaleOutEase).SetDelay(scaleOutDelay+toWalletDelay);

            DOTween.To(() => t, x => t = x, 1f, toWalletTime * durationMult).OnUpdate(() =>
            {
                SetViewportPosition(Vector3.Lerp(middlePos, endPos, t));
            }).SetEase(toWalletEase).OnComplete(() =>
            {
                if (this != null)
                {
                    onComplete?.Invoke();
                    pool.Pool(this);   
                }
            }).SetDelay(toWalletDelay);
        });
    }

    private void SetViewportPosition(Vector2 vp)
    {
        var sp = new Vector2(vp.x * Screen.width, vp.y * Screen.height);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, sp, null, out var lp);
        rectTransform.localPosition = lp;
    }

    public void OnReset()
    {
        
    }

    public void OnPool()
    {

    }
}