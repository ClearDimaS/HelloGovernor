using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WishesExtensions 
{
    public static EInteractable ToInteractable(this EWish type)
    {
        switch (type)
        {
            case EWish.Chat:
                return EInteractable.None;
            case EWish.Wander:
                return EInteractable.None;
            case EWish.Drinks:
                return EInteractable.Drink;
            case EWish.Flowers:
                return EInteractable.Flowers;
            case EWish.IceCream:
                return EInteractable.IceCream;
            default:
                throw new NotImplementedException($"wish type: {type} cant be converted to interactable!");
        }
    }
}
