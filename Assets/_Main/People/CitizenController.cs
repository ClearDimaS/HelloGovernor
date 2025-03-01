using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public abstract class CitizenBehaviour : MonoBehaviour
{
    public abstract void OnReset();
    public virtual void OnUpdate(bool visible)
    {

    }
}

public class CitizenController : CulledBehaviour
{
    [Inject] private EnvironmentManager environment;
    
    [SerializeField] private Walker walker;
    [SerializeField] private WishesController wishesController;
    [SerializeField] private Interactor interactor;

    private CitizenBehaviour[] behaviours;
    private HouseBuilding house;

    public Animator Animator { get; private set; }
    public bool IsChatting { get; set; }
    public Walker Walker => walker;
    public WishesController WishesController => wishesController;

    protected override void OnAwake()
    {
        base.OnAwake();
        Animator = GetComponentInChildren<Animator>();
        behaviours = GetComponentsInChildren<CitizenBehaviour>();
    }

    private void OnEnable()
    {
        foreach (var behaviour in behaviours)
        {
            behaviour.OnReset();
        }
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        foreach (var bhvr in behaviours)
        {
            bhvr.OnUpdate(visible);
        }
    }

    public void PlaceRandom()
    {
        if (!environment.IsReady)
        {
            UniTask.WaitUntil(() => environment.IsReady).ContinueWith(() =>
            {
                var pos = environment.GetRandomUnlockedPosition(0f);
                walker.Place(pos);
            });
            return;
        }
        var pos = environment.GetRandomUnlockedPosition(0f);
        walker.Place(pos);
    }

    public void Place(Vector3 citizenPlacePosition)
    {
        walker.Place(citizenPlacePosition);
    }

    public void AddItem(CitizenItem item)
    {
        interactor.AddItem(item);
        item.PoolPleaseAtTimeout(() =>
        {
            interactor.RemoveItem(item);
        });
    }

    public void PlayAnimation(string animName)
    {
        Animator.CrossFade(animName, 0.01f);
    }

    public void ResetAnimation()
    {
        Animator.CrossFade("Idle", 0.01f);
    }
}
