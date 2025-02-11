using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WaiterAssistant : MonoBehaviour, IWishAssistant, IItemTaker
{
    [SerializeField] private Interactor interactor;
    [SerializeField] private Walker walker;
    [SerializeField] private float rotSpeed = 360f;
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
            walker.MoveToTarget(itemsSource.IdlePlace.position, null);
            if (!walker.IsMoving)
            {
                var diff = itemsSource.TakePlace.position - transform.position;
                diff.y = 0f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(diff.normalized, Vector3.up),
                    rotSpeed * Time.deltaTime);
            }
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
        if (target != null && (!target.HasAssistant() || target.Assistant == this))
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
