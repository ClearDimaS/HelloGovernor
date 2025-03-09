using System;
using System.Collections;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public interface IWishAssistant
{
    public GenericCitizenItem RemoveItem();
    public bool HasAnyItems();
    public Transform TransformRoot { get; }
    public GenericCitizenItem PeekItem();
}

public class WishesController : CitizenBehaviour
{
    [Inject] private SoundManager soundManager;
    [Inject] private WishesPool wishesPool;
    [Inject] private WishGrantersManager grantersManager;

    [SerializeField] private CitizenController citizenController;

    private bool isWishesDisabled;
    private bool isPaused;
    private bool isWishOver;
    private Wish currentWish;
    private Transform target;
    
    private event Action<bool> wishResultEvent;
    
    public bool IsPaused => isPaused;

    public override void OnReset()
    {
        isWishesDisabled = false;
        currentWish = null;
    }

    public override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (currentWish == null && !isWishesDisabled)
        {
            SetRandomWish();
        }
    }

    public void AddProgress(WishGranter granter, float addProgress)
    {
        if (currentWish != null && currentWish.Granter == granter)
        {
            currentWish.AddProgress(addProgress);
        }
    }

    public bool IsProgressFull(WishGranter granter)
    {
        return currentWish != null && currentWish.Granter == granter && currentWish.IsProgressFull;
    }
    
    private void SetRandomWish()
    {
        if (isPaused)
        {
            return;
        }
        
        WishGranter granter = grantersManager.TryGetWorkingFreeGranter();
        if (granter != null)
        {
            CreateWishForGranter(granter);
        }
    }

    private void CreateWishForGranter(WishGranter granter)
    {
        isWishOver = false;
        currentWish = wishesPool.GetElement();
        currentWish.transform.SetParent(transform);
        currentWish.Initialize(citizenController, OnWishResult);
        currentWish.SetGranter(granter);
    }

    private void OnWishResult(Wish wish)
    {
        if (!isWishOver)
        {
            if (currentWish.IsSuccess && 
                wish.UseSound)
            {
                soundManager.WishDone();
            }
            isWishOver = true;
            wishResultEvent?.Invoke(currentWish.IsSuccess);   
        }
    }

    public void SubscribeWishesResult(Action<bool> handler)
    {
        wishResultEvent += handler;
    }

    public void DisableAllDesires()
    {
        isWishesDisabled = true;
    }
    
    public void EnableAllDesires()
    {
        isWishesDisabled = false;
    }

    public float GetProgress()
    {
        return currentWish == null ? 0f : currentWish.Progress;
    }

    public void RemoveWish(WishGranter wishGranter)
    {
        if (currentWish != null)
        {
            currentWish.SetRemoved(wishGranter);
            wishesPool.Pool(currentWish);
            currentWish = null;
        }
    }

    public void SetWish(WishGranter startWish)
    {
        CreateWishForGranter(startWish);
    }
}