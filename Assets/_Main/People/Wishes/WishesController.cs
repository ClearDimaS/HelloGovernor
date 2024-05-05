using System;
using System.Collections;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public interface IWishAssistant
{
    public bool CanServeType(EWish type);
}

public class WishesController : MonoBehaviour
{
    [Inject] private WishesPool wishesPool;
    [Inject] private WishGrantersManager grantersManager;
    [Inject] private GameConfig gameConfig;

    [SerializeField] private CitizenController citizenController;

    private IWishAssistant wishAssistant;
    public IWishAssistant WishAssistant => wishAssistant;
    private Wish currentWish;
    private Transform target;

    private void Update()
    {
        if (currentWish == null)
        {
            SetRandomWish();
        }

        if (currentWish != null && citizenController.CanAddWishes())
        {
            if (!currentWish.HasOKGranter())
            {
                if (grantersManager.TryGetWorkingFreeGranter(currentWish.Type, out WishGranter granter))
                {
                    currentWish.SetGranter(granter);   
                }
            }
        }
    }

    public bool HasAnyWish()
    {
        return currentWish != null;
    }
    
    public void AddProgress(EWish type, float addProgress)
    {
        if (currentWish != null && currentWish.Type == type)
        {
            currentWish.AddProgress(addProgress);
        }
    }

    public bool IsProgressFull(EWish type)
    {
        return currentWish != null && currentWish.Type == type && currentWish.IsProgressFull;
    }
    
    private void SetRandomWish()
    {
        var wishChances = gameConfig.wishChances;
        wishChances.Shuffle();
        var wishType = wishChances[Random.Range(0, wishChances.Count)].type;
        
        if (grantersManager.TryGetWorkingFreeGranter(wishType, out WishGranter granter) && granter.CanAdd(citizenController))
        {
            currentWish = wishesPool.GetElement();
            currentWish.transform.SetParent(transform);
            currentWish.Initialize(wishType, citizenController, PoolWish);
            currentWish.SetGranter(granter);
        }
    }

    private void PoolWish(Wish wish)
    {
        wishesPool.Pool(wish);
        currentWish = null;
    }

    public void AbortWish()
    {
        currentWish.Abort();
    }

    public bool IsGranterAssistantServing(EWish type)
    {
        switch (type)
        {
            case EWish.Drinks:
                return wishAssistant != null && wishAssistant.CanServeType(type);
            case EWish.Flowers:
                return wishAssistant != null && wishAssistant.CanServeType(type);
            case EWish.IceCream:
                return wishAssistant != null && wishAssistant.CanServeType(type);
            default:
                throw new NotImplementedException($"wish {type} cant be served!");
        }
    }

    public void SetWishAssistant(IWishAssistant waiterAssistant)
    {
        wishAssistant = waiterAssistant;
    }
}