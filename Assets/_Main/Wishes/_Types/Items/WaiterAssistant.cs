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
    private GenericCitizenItemSource itemsSource;
    
    private GenericCitizenItem item;
    private ItemsWishGranter wishGranter;
    private ItemsProcessPlace target;
    
    public Transform Root => transform;
    
    private void Awake()
    {
        wishGranter = GetComponentInParent<ItemsWishGranter>();
        itemsSource = wishGranter.GetComponentInChildren<GenericCitizenItemSource>();
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

            if (target.GetOwner() == null)
            {
                target = null;
            }
        }
    }

    public bool CanAddItems()
    {
        return item == null;
    }

    public void AddItem(GenericCitizenItem getElement)
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
    
    public GenericCitizenItem RemoveItem()
    {
        var removed = item;
        interactor.RemoveItem();
        item = null;
        return removed;
    }

    public bool HasAnyItems()
    {
        return item != null;
    }
    
    private void StartTakeItem()
    {
        itemsSource.AddTaker(this);
    }
}
