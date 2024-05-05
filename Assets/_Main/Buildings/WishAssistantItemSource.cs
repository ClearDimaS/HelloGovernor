using System;
using UnityEngine;
using Zenject;

public class WishAssistantItemSource : MonoBehaviour
{
    [Inject] private WishAssistantItemsPool pool;
    
    [field: SerializeField] public Transform TakePlace { get; private set; }
    private WishGranter wishGranter;

    private void Awake()
    {
        wishGranter = GetComponentInParent<WishGranter>();
    }

    public WishAssistantItem TakeItem()
    {
        return pool.GetElement(wishGranter.Type);
    }
}