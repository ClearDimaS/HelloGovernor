using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DrinkGranter : WishGranter
{
    protected override bool CanAddProgress(CitizenController citizen)
    {
        return citizen.WishesController.IsGranterAssistantServing(this);
    }

    protected override void OnSuccessProcess(CitizenController citizen)
    {
        base.OnSuccessProcess(citizen);
    }
}
