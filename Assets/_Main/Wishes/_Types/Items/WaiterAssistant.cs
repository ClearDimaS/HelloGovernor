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
    private ItemsProcessPlace target;
    
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
            if (target.HasAssistant() && target.Assistant != this)
            {
                target = null;
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
                target.RemoveAssistant(this);
                walker.MoveToTarget(itemsSource.TakePlace.position, StartTakeItem);
            }
            else
            {
                walker.MoveToTarget(target.ItemTakePlace.position, AllowAddProgressToWisher, 0.8f);   
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
        if (target != null && !target.HasAssistant())
        {
            target.SetWishAssistant(this);   
        }
        else
        {
            target = null;
        }
    }

    private void RefreshTarget()
    {
        target = wishGranter.GetProcessedWithoutAssistant();
    }
    
    public WishAssistantItem RemoveItem(ItemsWishGranter granter)
    {
        var removed = item;
        interactor.RemoveItem(removed.Type);
        item = null;
        return removed;
    }

    public bool HasItems(ItemsWishGranter granter)
    {
        return item != null;
    }
    
    private void StartTakeItem()
    {
        itemsSource.AddTaker(this);
    }
}
