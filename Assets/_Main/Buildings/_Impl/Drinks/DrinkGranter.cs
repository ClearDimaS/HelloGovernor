using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DrinkGranter : WishGranter
{
    public override EWish Type => EWish.Drinks;

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return citizen.WishesController.IsGranterAssistantServing(Type);
    }

    protected override void OnSuccessProcess(CitizenController citizen)
    {
        base.OnSuccessProcess(citizen);
    }
}
