using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WaiterAssistant : MonoBehaviour, IWishAssistant
{
    [Inject] private GameConfig config;
    
    [SerializeField] private Interactor interactor;
    [SerializeField] private Walker walker;
    private WishAssistantItemSource itemsSource;

    private bool canTakeItem;
    private float takeTimer;
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
        if (canTakeItem)
        {
            takeTimer += Time.deltaTime;
            if (takeTimer > config.takeItemTime)
            {
                takeTimer = 0f;
                canTakeItem = false;
                AddItem(itemsSource.TakeItem());
            }
        }
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
                walker.MoveToTarget(itemsSource.TakePlace.position, AllowTakeItem);
            }
            else
            {
                walker.MoveToTarget(target.transform.position, AllowAddProgress, 0.8f);   
            }
        }
    }

    public bool CanServeType(EWish type)
    {
        return this.type == type;
    }

    private void AllowTakeItem()
    {
        canTakeItem = true;
    }

    private void AllowAddProgress()
    {
        target.WishesController.SetWishAssistant(this);
    }

    private void RefreshTarget()
    {
        target = wishGranter.GetProcessedWithoutAssistant();
    }
    
    private void AddItem(WishAssistantItem getElement)
    {
        item = getElement;
        interactor.AddItem(item);
    }
}
