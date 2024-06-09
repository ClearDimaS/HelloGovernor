using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WishPlace : MonoBehaviour
{
    public enum EWishPlaceState
    {
        Empty,
        Owned,
        Taken
    }

    [Inject] private CompassManager compassManager;
    [Inject] private WishItemsConfig config;
        
    [SerializeField] private GameObject timerRoot;
    [SerializeField] private TimerBase timer;
    [SerializeField] private Image bgColor;
    [SerializeField] private Color noItemColor;

    private Color originalColor;
    private WishGranter granter;
    private CitizenController wisher;
    private EWishPlaceState state;
    public Vector3 Position => transform.position;

    private float progress => wisher.WishesController.CurrentWishProgress;
    public EWish Type => granter.Type;

    private ECompasTarget compasTarget;
    
    private void Awake()
    {
        granter = GetComponentInParent<WishGranter>();
        compasTarget = granter.Type.ToCompassTarget();
        timer.SetIcon(config.GetIcon(granter.Type));
        originalColor = bgColor.color;
    }

    private void Update()
    {
        if (timer != null && wisher != null && state == EWishPlaceState.Taken)
        {
            if (!timerRoot.activeSelf)
            {
                timerRoot.SetActive(true);
                compassManager.AddTarget(transform, compasTarget);
            }
            timer.SetProgress(progress);   
        }
        else
        {
            if (timerRoot.activeSelf)
            {
                compassManager.RemoveTarget(transform, compasTarget);
                timerRoot.SetActive(false);
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
}