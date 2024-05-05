using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DrinkGranter : WishGranter
{
    [Inject] private GameConfig gameConfig;
    public override EWish Type => EWish.Drinks;
    public override float FullProgressTime => gameConfig.grantDrinkDuration;
    public override int Reward => gameConfig.drinkReward;

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return citizen.WishesController.IsGranterAssistantServing(Type);
    }

    protected override void OnSuccessProcess(CitizenController citizen)
    {
        base.OnSuccessProcess(citizen);
    }
}
