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
    
    public static ECompasTarget ToCompassTarget(this EWish type)
    {
        switch (type)
        {
            case EWish.Drinks:
                return ECompasTarget.Drink;
            case EWish.Flowers:
                return ECompasTarget.Flower;
            case EWish.IceCream:
                return ECompasTarget.IceCream;
            default:
                throw new NotImplementedException($"wish type: {type} cant be converted to interactable!");
        }
    }

    public static bool IsCompassTarget(this EWish type)
    {
        return type == EWish.Drinks || type == EWish.Flowers || type == EWish.IceCream;
    }
}
