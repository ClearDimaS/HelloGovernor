using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OrderWishDisplayer : CulledBehaviour
{
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private Transform content;
    protected bool isShown;
    protected OrderProcessPlace processPlace;

    protected override void OnAwake()
    {
        base.OnAwake();
        processPlace = GetComponentInParent<OrderProcessPlace>();
        content.transform.localScale = Vector3.zero;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        var show = processPlace.IsAtPlace();
        if (show != isShown)
        {
            if (show)
            {
                content.DOKill();
                content.DOScale(Vector3.one * 1.2f, 0.3f).OnComplete(() =>
                {
                    content.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear);
                }).SetEase(Ease.Linear);
            }
            else
            {
                content.DOKill();
                content.DOScale(Vector3.zero, 0.3f);
            }
        }

        if (show)
        {
            var curIcon = processPlace.GetCurrentIcon();
            var curColor = processPlace.GetCurrentColor();
            if (icon.sprite != curIcon || icon.color.r != curColor.r 
                                       || icon.color.g != curColor.g
                                       || icon.color.b != curColor.b)
            {
                icon.sprite = curIcon;
                icon.color = curColor;
            }
        }
    }
}