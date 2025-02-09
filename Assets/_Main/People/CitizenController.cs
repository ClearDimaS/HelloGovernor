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
    public bool IsChatting { get; private set; }
    public Walker Walker => walker;
    public WishesController WishesController => wishesController;

    protected override void OnAwake()
    {
        base.OnAwake();
        Animator = GetComponentInChildren<Animator>();
        behaviours = GetComponentsInChildren<CitizenBehaviour>();
    }

    public void SetHouse(HouseBuilding house)
    {
        this.house = house;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (house != null && house.IsBroken())
        {
            if (wishesController.HasAnyWish())
            {
                wishesController.AbortWish();
            }
            walker.MoveToTarget(house.CitizenPlace.position, null);
        }

        foreach (var bhvr in behaviours)
        {
            bhvr.OnUpdate(visible);
        }
    }

    private Vector3 GetRandomPos()
    {
        var pos = environment.mapCenter + new Vector3(
            Random.Range(-environment.mapSize.x / 2f, environment.mapSize.x / 2f),
            0,
            Random.Range(-environment.mapSize.z / 2f, environment.mapSize.z / 2f));
        return pos;
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

    public void SetChatting()
    {
        IsChatting = true;
    }

    public void StopChatting()
    {
        IsChatting = false;
    }

    public bool CanAddWishes()
    {
        return house == null || !house.IsBroken();
    }

    public void AddItem(WishAssistantItem item)
    {
        if (!interactor.HasMorePlaceFor(item.Type))
        {
            return;
        }
        interactor.AddItem(item);
        var type = item.Type;
        var time = 10f;
        UniTask.Delay(TimeSpan.FromSeconds(time)).ContinueWith(() =>
        {
            interactor.RemoveItem(type);
        });
    }

    public bool HasItem(WishGranter type)
    {
        return interactor.HasItem(type);
    }
}
