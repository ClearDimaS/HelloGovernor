using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public abstract class CitizenBehaviour : MonoBehaviour
{
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

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        foreach (var bhvr in behaviours)
        {
            bhvr.OnUpdate(visible);
        }
    }

    private Vector3 GetRandomPos()
    {
        return environment.GetRandomUnlockedPosition(0f);
    }

    public void PlaceRandom()
    {
        var pos = GetRandomPos();
        walker.Place(pos);
    }

    public void Place(Vector3 citizenPlacePosition)
    {
        walker.Place(citizenPlacePosition);
    }

    public void AddItem(CitizenItem item)
    {
        interactor.AddItem(item);
        var time = 10f;
        UniTask.Delay(TimeSpan.FromSeconds(time)).ContinueWith(() =>
        {
            interactor.RemoveItem();
            item.PoolPlease();
        });
    }
}
