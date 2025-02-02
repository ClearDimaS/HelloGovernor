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
        for (var i = 0; i < takenPlaces.Count; i++)
        {
            var client = takenPlaces[i].GetWisher();
            if (client == null || !CanServeType(takenPlaces[i].Type))
            {
                continue;
            }
            if (client.WishesController.CurrentWishProgress >= 1f)
            {
                var give = interactor.RemoveItem(takenPlaces[i].Type);
                client.AddItem(give);
                StopServingClient(client);
            }else if(client.WishesController.WishType == takenPlaces[i].Type)
            {
                client.WishesController.SetWishAssistant(this);   
            }
        }
    }

    protected override void OnEnter(WishPlace place)
    {
        base.OnEnter(place);
        if (!takenPlaces.Contains(place))
        {
            takenPlaces.Add(place);
        }
        
        if (!place.TryGetWisher(out CitizenController citizen))
        {
            return;
        }

        if (citizen.WishesController.CurrentWishProgress <= 1f)
        {
            if (!CanServeType(place.Type))
            {
                place.NotiftyError();
            }
        }
    }

    protected override void OnLeave(WishPlace place)
    {
        base.OnLeave(place);
        takenPlaces.Remove(place);
        if (place.TryGetWisher(out var wisher) && wisher.WishesController.WishAssistant == this)
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
        client.WishesController.SetWishAssistant(null);
    }
}