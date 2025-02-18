using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class BankGranter : UIWishGranter
{
    [SerializeField] protected float gameDuration = 30f;
    protected BankGame game;
    protected override UI_Panel GetPanel()
    {
        return UI_Manager.Instance.GetPanel<BankWishPanel>();
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        game = new BankGame(gameDuration);
        (panel as BankWishPanel).Init(game);
    }
}