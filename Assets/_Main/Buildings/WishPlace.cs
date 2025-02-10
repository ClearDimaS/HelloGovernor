using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

public class WishPlace : CulledBehaviour
{
    public enum EWishPlaceState
    {
        Empty,
        Owned,
        Taken
    }

    [SerializeField] private GameObject timerRoot;
    [SerializeField] private TimerBase timer;
    [SerializeField] private Image bgColor;
    [SerializeField] private GameObject placeRoot;
    [SerializeField] private Color noItemColor;
    
    private Color originalColor;
    private ItemsWishGranter granter;
    private CitizenController wisher;
    private EWishPlaceState state;
    public Vector3 Position => transform.position;

    private float progress => wisher.WishesController.CurrentWishProgress;
    public ItemsWishGranter Granter => granter;

    protected override void OnAwake()
    {
        base.OnAwake();
        granter = GetComponentInParent<ItemsWishGranter>();
        timer.SetIcon(granter.GetItemIcon());
        originalColor = bgColor.color;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (timer != null && wisher != null && state == EWishPlaceState.Taken)
        {
            if (!timerRoot.activeSelf)
            {
                timerRoot.SetActive(true);
            }

            if (!placeRoot.activeSelf)
            {
                placeRoot.SetActive(true);
            }
            timer.SetProgress(progress);   
        }
        else
        {
            if (timerRoot.activeSelf)
            {
                timerRoot.SetActive(false);
            }
            if (placeRoot.activeSelf)
            {
                placeRoot.SetActive(false);
            }
        }
    }

    public void SetOwner(CitizenController wisher)
    {
        this.wisher = wisher;
        state = EWishPlaceState.Owned;
    }
    
    public void TakePlace(CitizenController wisher)
    {
        this.wisher = wisher;
        state = EWishPlaceState.Taken;
    }

    public void LeavePlace(CitizenController wisher)
    {
        if (this.wisher == wisher)
        {
            this.wisher = null;
            state = EWishPlaceState.Empty;
        }
    }

    public bool TryGetWisher(out CitizenController outVal)
    {
        outVal = this.wisher;
        return outVal != null;
    }

    public void NotiftyError()
    {
        bgColor.DOKill();
        bgColor.transform.DOKill();
        bgColor.transform.DOScale(1.2f, 0.2f).OnComplete(() =>
        {
            bgColor.transform.DOScale(1f, 0.2f);
        });
        bgColor.DOColor(noItemColor, 0.2f).OnComplete(() =>
        {
            bgColor.DOColor(originalColor, 0.2f);
        });
    }

    public CitizenController GetWisher()
    {
        return wisher;
    }
}