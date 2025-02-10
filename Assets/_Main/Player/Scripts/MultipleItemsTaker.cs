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
            if (client == null || !CanServeType(takenPlaces[i].Granter))
            {
                continue;
            }
            if (client.WishesController.CurrentWishProgress >= 1f)
            {
                var give = interactor.RemoveItem(takenPlaces[i].Granter);
                client.AddItem(give);
                StopServingClient(client);
            }else if(client.WishesController.WishGranter == takenPlaces[i].Granter)
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
            if (!CanServeType(place.Granter))
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

    public bool CanAddItems(WishGranter granter)
    {
        return interactor.HasMorePlaceFor(granter);
    }

    public void AddItem(WishAssistantItem item)
    {
        SoundManager.Instance.PlayerTakeItem();
        interactor.AddItem(item);
    }

    public bool CanServeType(WishGranter granter)
    {
        return interactor.HasItem(granter);
    }

    private void StopServingClient(CitizenController client)
    {
        client.WishesController.SetWishAssistant(null);
    }
}