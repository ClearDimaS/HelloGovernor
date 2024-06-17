using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MultipleItemsTaker : SimpleWishPlacePhysicsBehaviour, IItemTaker, IWishAssistant
{
    private Interactor interactor;
    private CitizenController client;
    private WishPlace takenPlace;

    public Transform Root => transform;
    
    protected override void OnAwake()
    {
        base.OnAwake();
        interactor = GetComponent<Interactor>();
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (client != null)
        {
            if (client.WishesController.CurrentWishProgress >= 1f)
            {
                var give = interactor.RemoveItem(client.WishesController.WishType);
                client.AddItem(give);
                
                StopServingClient();
            }
        }
    }

    protected override void OnEnter(WishPlace place)
    {
        base.OnEnter(place);
        if (takenPlace != null)
        {
            return;
        }
        if (!place.TryGetWisher(out CitizenController citizen))
        {
            return;
        }

        if (citizen.WishesController.CurrentWishProgress <= 1f)
        {
            if (CanServeType(place.Type))
            {
                takenPlace = place;
                this.client = citizen;
                client.WishesController.SetWishAssistant(this);   
            }
            else
            {
                place.NotiftyError();
            }
        }
    }

    protected override void OnLeave(WishPlace place)
    {
        base.OnLeave(place);
        if (takenPlace == place && takenPlace.TryGetWisher(out var wisher))
        {
            StopServingClient();
        }
    }

    public bool CanAddItems(EInteractable type)
    {
        return interactor.HasMorePlaceFor(type);
    }

    public void AddItem(WishAssistantItem item)
    {
        SoundManager.Instance.PlayerTakeItem();
        interactor.AddItem(item);
    }

    public bool CanServeType(EWish type)
    {
        return interactor.HasItem(type);
    }

    private void StopServingClient()
    {
        takenPlace = null;
        var tmp = client;
        client = null;
        tmp.WishesController.SetWishAssistant(null);
    }
}