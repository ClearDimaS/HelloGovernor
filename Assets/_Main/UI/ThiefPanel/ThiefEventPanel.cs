using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ThiefEventPanel : UI_Panel
{
    [SerializeField] private UI_ElementAnimator timerContent;
    [SerializeField] private UIWishTimer uiWishTimer;
    
    protected PolicestationBuilding policeBuilding;
    protected override void OnAwake()
    {
        base.OnAwake();
        policeBuilding = FindObjectOfType<PolicestationBuilding>(true);
        timerContent.AnimateBack(instant:true);
    }

    private void Update()
    {
        var hasThief = policeBuilding.HasThief();
        if (hasThief != timerContent.IsShown)
        {
            if (hasThief)
            {
                timerContent.Animate();
            }
            else
            {
                timerContent.AnimateBack();
            }
        }

        if (hasThief)
        {
            uiWishTimer.SetTimeLeft(policeBuilding.GetTimeLeft());
        }
    }
}