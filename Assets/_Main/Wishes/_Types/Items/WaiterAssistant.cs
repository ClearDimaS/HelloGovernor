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
    private ItemsWishGranter wishGranter;
    private CitizenController target;
    
    public Transform Root => transform;
    
    private void Awake()
    {
        wishGranter = GetComponentInParent<ItemsWishGranter>();
        itemsSource = wishGranter.GetComponentInChildren<WishAssistantItemSource>();
    }

    private void Update()
    {
        if (target != null)
        {
            if (target.WishesController.WishAssistant != null && target.WishesController.WishAssistant != this)
            {
                target = null;
            }
        }

        if (target != null && item != null)
        {
            if (!wishGranter.IsProcessed(target))
            {
                var removed = item;
                target.WishesController.SetWishAssistant(null);
                interactor.RemoveItem(removed.Type);
                target.AddItem(removed);
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

    public bool CanServeType(WishGranter granter)
    {
        return this.wishGranter == granter;
    }

    public bool CanAddItems(ItemsWishGranter granter)
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
        if (target != null)
        {
            target.WishesController.SetWishAssistant(this);   
        }
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
