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
    [Inject] private WishesConfig wishesConfig;

    [SerializeField] private CitizenController citizenController;

    private IWishAssistant wishAssistant;
    public IWishAssistant WishAssistant => wishAssistant;

    public bool IsSitting => currentWish != null && (currentWish.Type == EWish.Drinks ||
                                                    currentWish.Type == EWish.Flowers ||
                                                    currentWish.Type == EWish.IceCream) && IsProcessingWish;
    public bool IsProcessingWish => currentWish != null && currentWish.Granter != null && currentWish.Granter.IsProcessed(citizenController);
    public float CurrentWishProgress =>  currentWish != null ? currentWish.Progress : -1f;
    public EWish WishType => currentWish == null ? EWish.Wander : currentWish.Type;
    
    private event Action<bool> wishResultEvent;
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
        var wishType = wishesConfig.GetRandomWishType();
        
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
        wishResultEvent?.Invoke(currentWish.IsSuccess);
        wishesPool.Pool(wish);
        currentWish = null;
    }

    public void AbortWish()
    {
        currentWish.Abort();
        wishResultEvent?.Invoke(currentWish.IsSuccess);
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

    public void SubscribeWishesResult(Action<bool> handler)
    {
        wishResultEvent += handler;
    }
}