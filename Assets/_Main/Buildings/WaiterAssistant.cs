using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiterAssistant : MonoBehaviour, IWishAssistant
{
    [SerializeField] private Walker walker;
    
    private EWish type;
    private WishGranter wishGranter;
    private CitizenController target;

    private void Awake()
    {
        wishGranter = GetComponentInParent<WishGranter>();
        type = wishGranter.Type;
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
                target = null;
            }
        }

        if (target == null)
        {
            RefreshTarget();
        }
        if (target != null)
        {
            walker.MoveToTarget(target.transform.position, AllowAddProgress);
        }
    }

    private void AllowAddProgress()
    {
        target.WishesController.SetWishAssistant(this);
    }

    private void RefreshTarget()
    {
        target = wishGranter.GetProcessedWithoutAssistant();
    }

    public bool CanServeType(EWish type)
    {
        return this.type == type;
    }
}
