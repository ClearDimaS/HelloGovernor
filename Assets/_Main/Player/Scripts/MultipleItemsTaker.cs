using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MultipleItemsTaker : MonoBehaviour, IItemTaker, IWishAssistant
{
    private Interactor interactor;
    private CitizenController client;

    private void Awake()
    {
        interactor = GetComponent<Interactor>();
    }

    private void Update()
    {
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger || other.attachedRigidbody == null)
        {
            return;
        }

        var otherRb = other.attachedRigidbody;
        if (!otherRb.TryGetComponent(out WishPlace place))
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
                this.client = citizen;
                client.WishesController.SetWishAssistant(this);   
            }
            else
            {
                place.NotiftyError();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger || other.attachedRigidbody == null)
        {
            return;
        }

        var otherRb = other.attachedRigidbody;
        if (!otherRb.TryGetComponent(out WishPlace place))
        {
            return;
        }
        if (!place.TryGetWisher(out CitizenController citizen))
        {
            return;
        }

        if (this.client == citizen)
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
        client.WishesController.SetWishAssistant(null);
        client = null;
    }
}