using System;
using UnityEngine;

[Serializable]
public class Wish : MonoBehaviour, IResetable
{
    public EWish Type { get; private set; }
    public bool IsReadyToRemove { get; private set; }
    public bool IsProgressFull => progress >= 1f;

    private CitizenController citizen;
    private Walker walker;
    
    private WishGranter granter;
    private float progress;
    
    public void Initialize(EWish type, CitizenController citizen)
    {
        Type = type;
        this.citizen = citizen;
        walker = citizen.Walker;
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

        if (!granter.HasInQueueOrProcessed(citizen) &&  progress == 0f)
        {
            granter.AddApproaching(citizen);
        }
    }

    private void SetWishReadyToRemove()
    {
        IsReadyToRemove = true;
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
        granter = null;
        citizen = null;
        walker = null;
        IsReadyToRemove = false;
    }

    public void OnPool()
    {
        
    }

    public void AddProgress(float addProgress)
    {
        progress += addProgress;
    }
}