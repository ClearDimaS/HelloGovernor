using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WaiterAssistant : MonoBehaviour, IWishAssistant, IItemTaker
{
    [SerializeField] private Interactor interactor;
    [SerializeField] private Walker walker;
    private WishAssistantItemSource itemsSource;
    
    private WishAssistantItem item;
    private EWish type;
    private WishGranter wishGranter;
    private CitizenController target;
    
    private void Awake()
    {
        wishGranter = GetComponentInParent<WishGranter>();
        type = wishGranter.Type;
        itemsSource = wishGranter.GetComponentInChildren<WishAssistantItemSource>();
    }

    private void Update()
    {
        if (target != null)
        {
            if (target.WishesController.WishAssistant != null && target.WishesController.WishAssistant != this)
            {
                target.WishesController.SetWishAssistant(null);
                target = null;
            }
        }

        if (target != null)
        {
            if (!wishGranter.IsProcessed(target))
            {
                target.WishesController.SetWishAssistant(null);
                interactor.RemoveItem(item);
                target.Interactor.AddItem(item);
                target = null;
                item = null;
            }
        }

        if (target == null)
        {
            RefreshTarget();
        }
        if (target != null)
        {
            if (item == null)
            {
                walker.MoveToTarget(itemsSource.TakePlace.position, StartTakeItem);
            }
            else
            {
                walker.MoveToTarget(target.transform.position, AllowAddProgressToWisher, 0.8f);   
            }
        }
    }

    public bool CanServeType(EWish type)
    {
        return this.type == type;
    }

    public bool CanAddItems(EInteractable type)
    {
        return item == null;
    }

    public void AddItem(WishAssistantItem getElement)
    {
        item = getElement;
        interactor.AddItem(item);
    }
    
    private void AllowAddProgressToWisher()
    {
        target.WishesController.SetWishAssistant(this);
    }

    private void RefreshTarget()
    {
        target = wishGranter.GetProcessedWithoutAssistant();
    }
    
    private void StartTakeItem()
    {
        itemsSource.AddTaker(this);
    }
}
