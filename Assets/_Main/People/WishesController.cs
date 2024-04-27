using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[Serializable]
public class Wish
{
    public EWish Type { get; private set; }

    public Wish(EWish type)
    {
        Type = type;
    }
}

public enum EWish
{
    Wander,
    Drinks,
    IceCream,
    Flowers,
    Chat
}

public class WishGranter
{
    
}

public class WishesGranterManager
{
    
}

public class WishesController : MonoBehaviour
{
    [Inject] private GameConfig gameConfig;

    private Wish currentWish;
    private Transform target;

    private void Update()
    {
        if (currentWish == null)
        {
            SetRandomWish();
        }

        if (currentWish != null)
        {
            
        }
    }

    private void SetRandomWish()
    {
        var wishChances = gameConfig.wishChances;
        wishChances.Shuffle();
        var wishType = wishChances[0].type;
        currentWish = new Wish(wishType);
    }
}
