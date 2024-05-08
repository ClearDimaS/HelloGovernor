using System;
using UnityEngine;
using UnityEngine.UI;

public class WishPlace : MonoBehaviour
{
    public enum EWishPlaceState
    {
        Empty,
        Owned,
        Taken
    }

    [SerializeField] private Image fillImage;
    
    private CitizenController wisher;
    private EWishPlaceState state;
    public Vector3 Position => transform.position;

    private float progress => wisher.WishesController.CurrentWishProgress;

    private void Update()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = progress;   
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
}