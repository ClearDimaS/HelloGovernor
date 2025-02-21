using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OrderWishDisplayer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private Transform content;
    protected bool isShown;
    protected OrderProcessPlace processPlace;
    private void Awake()
    {
        processPlace = GetComponentInParent<OrderProcessPlace>();
    }

    private void Update()
    {
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
    }

    public void Init(OrderItemsConfigData orderDataItem)
    {
        icon.sprite = orderDataItem.itemIcon;
    }
}