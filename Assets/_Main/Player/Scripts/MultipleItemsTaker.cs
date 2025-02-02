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
    private List<CitizenController> clients = new ();
    private List<WishPlace> takenPlaces = new ();

    public Transform Root => transform;
    
    protected override void OnAwake()
    {
        base.OnAwake();
        interactor = GetComponent<Interactor>();
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        for (var i = 0; i < clients.Count; i++)
        {
            var client = clients[i];
            if (client.WishesController.CurrentWishProgress >= 1f)
            {
                var give = interactor.RemoveItem(client.WishesController.WishType);
                client.AddItem(give);

                StopServingClient(client);
            }
        }
    }

    protected override void OnEnter(WishPlace place)
    {
        base.OnEnter(place);
        if (!place.TryGetWisher(out CitizenController citizen))
        {
            return;
        }

        if (citizen.WishesController.CurrentWishProgress <= 1f)
        {
            if (CanServeType(place.Type))
            {
                if (!takenPlaces.Contains(place))
                {
                    takenPlaces.Add(place);
                    clients.Add(citizen);
                    citizen.WishesController.SetWishAssistant(this);   
                }
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
        if (takenPlaces.Remove(place) && place.TryGetWisher(out var wisher) && clients.Contains(wisher))
        {
            StopServingClient(wisher);
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

    private void StopServingClient(CitizenController client)
    {
        var tmp = client;
        client = null;
        clients.Remove(tmp);
        tmp.WishesController.SetWishAssistant(null);
    }
}