using System;
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
    [SerializeField] private Image fillImage;
    [SerializeField] private Image icon;
    
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
        icon.sprite = config.GetIcon(granter.Type);
    }

    private void Update()
    {
        if (fillImage != null && wisher != null && state == EWishPlaceState.Taken)
        {
            if (!timerRoot.activeSelf)
            {
                timerRoot.SetActive(true);
                compassManager.AddTarget(transform, compasTarget);
            }
            fillImage.fillAmount = progress;   
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
}