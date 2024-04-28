using System;
using UnityEngine;

[Serializable]
public class Wish : MonoBehaviour, IResetable
{
    private CitizenController citizen;
    private Walker walker;
    private WishGranter granter;

    private float progress;
    private event Action<Wish> removeEvent;
    
    public EWish Type { get; private set; }
    public bool IsReadyToRemove { get; private set; }
    public bool IsProgressFull => progress >= 1f;
    
    public void Initialize(EWish type, CitizenController citizen, Action<Wish> onRemove)
    {
        Type = type;
        this.citizen = citizen;
        walker = citizen.Walker;
        removeEvent += onRemove;
    }

    private void Update()
    {
        if (granter == null)
        {
            return;
        }

        if (progress >= 1f && !granter.HasInQueueOrProcessed(citizen))
        {
            SetWishReadyToRemove();
        }

        if (!granter.HasInQueueOrProcessed(citizen) && progress == 0f)
        {
            granter.AddApproaching(citizen);
        }
    }
    public bool HasOKGranter()
    {
        return granter != null && granter.IsWorking();
    }

    public void SetGranter(WishGranter granter)
    {
        this.granter = granter;
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
        IsReadyToRemove = true;
        SetRemoved();
    }

    public void Abort()
    {
        granter.Abort(citizen);
        SetRemoved();
    }
    
    private void SetRemoved()
    {
        removeEvent?.Invoke(this);
    }
}