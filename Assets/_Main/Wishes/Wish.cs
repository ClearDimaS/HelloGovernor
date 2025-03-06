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

    public bool IsRemoved { get; private set; }
    public bool IsProgressFull => progress >= 1f;
    public WishGranter Granter => granter;
    public float Progress => progress;

    public bool IsSuccess => progress >= 1f;
    public bool UseSound { get; private set; }

    public void Initialize(CitizenController citizen, Action<Wish> onReadyToRemove)
    {
        progress = 0;
        this.citizen = citizen;
        walker = citizen.Walker;
        readyToRemoveEvent += onReadyToRemove;
        IsRemoved = false;
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
    }

    public void OnPool()
    {
        
    }

    public void AddProgress(float addProgress)
    {
        progress += addProgress;
    }

    /*
    public void Abort()
    {
        granter.Abort(citizen);
        SetRemoved(granter);
    }
    */
    
    public void SetRemoved(WishGranter granter)
    {
        if (granter != this.granter)
        {
            Debug.LogError($"removing other wish!  {granter}  /  {this.granter}");
        }
        if (!IsRemoved)
        {
            var fire = readyToRemoveEvent;
            fire?.Invoke(this);
            readyToRemoveEvent = null;
            
            IsRemoved = true;
        }
    }
}