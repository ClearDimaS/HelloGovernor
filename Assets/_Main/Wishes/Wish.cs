using System;
using UnityEngine;

[Serializable]
public class Wish : UpdateableBehaviour, IResetable
{
    private CitizenController citizen;
    private Walker walker;
    private WishGranter granter;

    private float progress;
    private event Action<Wish> readyToRemoveEvent;
    private event Action<Wish> removeEvent;

    public bool IsReadyToRemove { get; private set; }
    public bool IsRemoved { get; private set; }
    public bool IsProgressFull => progress >= 1f;
    public WishGranter Granter => granter;
    public float Progress => progress;

    public bool IsSuccess => progress >= 1f;
    public bool UseSound { get; private set; }

    public void Initialize(CitizenController citizen, Action<Wish> onRemove, Action<Wish> onReadyToRemove)
    {
        progress = 0;
        this.citizen = citizen;
        walker = citizen.Walker;
        removeEvent += onRemove;
        readyToRemoveEvent += onReadyToRemove;
        IsRemoved = false;
        IsReadyToRemove = false;
    }

    public override void UpdateCall(float deltaTime)
    {
        base.UpdateCall(deltaTime);
        if (granter == null)
        {
            return;
        }

        if (progress >= 1)
        {
            SetWishReadyToRemove();
        }
        if (progress >= 1f && !granter.HasInQueueOrProcessed(citizen))
        {
            SetRemoved();
        }
    }

    public bool HasOKGranter()
    {
        return granter != null && granter.IsWorking();
    }

    public void SetGranter(WishGranter granter)
    {
        this.granter = granter;
        granter.AddApproaching(citizen);
    }

    public void OnReset()
    {
        progress = 0f;
        granter = null;
        citizen = null;
        walker = null;
        IsReadyToRemove = false;
        removeEvent = null;
    }

    public void OnPool()
    {
        
    }

    public void AddProgress(float addProgress)
    {
        progress += addProgress;
    }
    
    private void SetWishReadyToRemove()
    {
        if (!IsReadyToRemove)
        {
            IsReadyToRemove = true;
            readyToRemoveEvent?.Invoke(this);
        }
    }

    public void Abort()
    {
        granter.Abort(citizen);
        SetRemoved();
    }
    
    private void SetRemoved()
    {
        if (!IsRemoved)
        {
            IsRemoved = true;
            removeEvent?.Invoke(this);
        }
    }
}