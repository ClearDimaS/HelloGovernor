using System;
using System.Collections;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public interface IWishAssistant
{
    public bool CanServeType(WishGranter type);
}

public class WishesController : CitizenBehaviour
{
    [Inject] private PlayerController player;
    [Inject] private WishesPool wishesPool;
    [Inject] private WishGrantersManager grantersManager;
    [Inject] private WishesConfig wishesConfig;

    [SerializeField] private CitizenController citizenController;

    private bool isPaused;
    private bool isWishOver;
    private IWishAssistant wishAssistant;
    private Wish currentWish;
    private Transform target;
    
    private event Action<bool> wishResultEvent;
    
    public bool IsPaused => isPaused;
    public IWishAssistant WishAssistant => wishAssistant;
    public bool IsProcessingWish => currentWish != null && currentWish.Granter != null && currentWish.Granter.IsProcessed(citizenController);
    public float CurrentWishProgress =>  currentWish != null ? currentWish.Progress : -1f;
    public WishGranter WishGranter => currentWish.Granter;

    public override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (currentWish == null)
        {
            wishAssistant = null;
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
    
    public void AddProgress(WishGranter type, float addProgress)
    {
        if (currentWish != null && currentWish.Type == type.GetType())
        {
            currentWish.AddProgress(addProgress);
        }
    }

    public bool IsProgressFull(WishGranter type)
    {
        return currentWish != null && currentWish.Type == type.GetType() && currentWish.IsProgressFull;
    }
    
    private void SetRandomWish()
    {
        if (isPaused)
        {
            return;
        }
        
        var moneyWishType = wishesConfig.GetRandomMoneyWishType();
        if (grantersManager.TryGetWorkingFreeGranter(moneyWishType, out WishGranter granter) && granter.CanAdd(citizenController))
        {
            isWishOver = false;
            currentWish = wishesPool.GetElement();
            currentWish.transform.SetParent(transform);
            currentWish.Initialize(moneyWishType, citizenController, PoolWish, OnWishResult);
            currentWish.SetGranter(granter);
        }
        else
        {
            var wishType = wishesConfig.GetRandomWishType();
        
            if (grantersManager.TryGetWorkingFreeGranter(wishType, out granter) && granter.CanAdd(citizenController))
            {
                isWishOver = false;
                currentWish = wishesPool.GetElement();
                currentWish.transform.SetParent(transform);
                currentWish.Initialize(wishType, citizenController, PoolWish, OnWishResult);
                currentWish.SetGranter(granter);
            }
        }
    }

    private void PoolWish(Wish wish)
    {
        wishesPool.Pool(wish);
        currentWish = null;
    }
    
    private void OnWishResult(Wish wish)
    {
        if (!isWishOver)
        {
            if (currentWish.IsSuccess && 
                wish.UseSound && wishAssistant == player.WishAssistant)
            {
                SoundManager.Instance.WishDone();
            }
            isWishOver = true;
            wishResultEvent?.Invoke(currentWish.IsSuccess);   
        }
    }

    public void AbortWish()
    {
        if (currentWish != null)
        {
            var success = currentWish.IsSuccess;
            currentWish.Abort();
            if (!isWishOver)
            {
                isWishOver = true;
                wishResultEvent?.Invoke(success);
            }   
        }
    }

    public bool IsGranterAssistantServing(WishGranter type)
    {
        return wishAssistant != null && wishAssistant.CanServeType(type);
    }

    public void SetWishAssistant(IWishAssistant waiterAssistant)
    {
        wishAssistant = waiterAssistant;
    }

    public void SubscribeWishesResult(Action<bool> handler)
    {
        wishResultEvent += handler;
    }

    public void Pause()
    {
        isPaused = true;
        AbortWish();
    }
    
    public void UnPause()
    {
        isPaused = false;
    }
}