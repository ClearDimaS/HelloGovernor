using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DrinkGranter : ItemsWishGranter
{
    protected override bool CanAddProgress(CitizenController citizen)
    {
        return citizen.WishesController.IsGranterAssistantServing(this);
    }
}
