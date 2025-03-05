using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class BankGranter : UIWishGranter
{
    [Inject] protected WishesCollectionConfig wishesConfig;

    protected override bool CanAddToStarted => true;
    protected BankGame game;
    protected float lastAnswerTime;
    protected float minPause = 0.2f;
    
    protected override UI_Panel GetPanel()
    {
        return UI_Manager.Instance.GetPanel<BankWishPanel>();
    }

    protected override void OnActivate()
    {
        game = new BankGame(timer as WishGranterGameTimer, wishesConfig, GiveReward);
        (panel as BankWishPanel).Init(game, () =>
            {
                processPlaces[0].SetComplete();
                var owner = processPlaces[0].GetOwner();
                if (owner != null)
                {
                    lastAnswerTime = Time.time;
                    owner.WishesController.AddProgress(this, 1);
                }
            },
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
        AddMoney(count);
    }
}