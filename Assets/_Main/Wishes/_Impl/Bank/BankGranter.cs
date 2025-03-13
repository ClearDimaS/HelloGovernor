using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class BankGranter : UIWishGranter
{
    [Inject] protected CompassManager compassManager;
    [Inject] protected WishesCollectionConfig wishesConfig;

    protected override bool CanAddToStarted => true;
    protected float lastAnswerTime;
    protected float minPause = 0.2f;

    protected override UI_Panel GetPanel()
    {
        return UI_Manager.Instance.GetPanel<BankWishPanel>();
    }

    protected override void ShowActivationPlace()
    {
        base.ShowActivationPlace();
        compassManager.AddTarget(activationPlace.transform, ECompasTarget.Bank);
    }

    protected override void HideActivationPlace()
    {
        base.HideActivationPlace();
        compassManager.RemoveTarget(activationPlace.transform, ECompasTarget.Bank);
    }

    protected override void OnActivate()
    {
        var game = new BankGame(coolDownTimer.GetGameTimer(), wishesConfig, GiveReward, onAnswer: () =>
        {
            var owner = processPlaces[0].GetOwner();
            lastAnswerTime = Time.time;
            owner.WishesController.AddProgress(this, 1);
        });
        
        (panel as BankWishPanel).Init(game,
            () =>
            {
                return (Time.time - lastAnswerTime > minPause) && 
                       processPlaces[0].GetOwner() != null && 
                       !processPlaces[0].GetOwner().Walker.IsMoving;
            });
        base.OnActivate();
    }
    
    private void GiveReward(int count)
    {
        AddMoney(count, processPlaces[0].Position + Vector3.up);
    }
}