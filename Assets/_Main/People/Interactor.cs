using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public enum EInteractable
{
    None,
    Drink,
    Flowers,
    IceCream
}
public class Interactor : MonoBehaviour
{
    private Animator controller;
    private List<WishAssistantItem> items = new ();
    
    private void Awake()
    {
        controller = GetComponentInChildren<Animator>();
    }
    
    public bool HasItem(EWish type)
    {
        switch (type)
        {
            case EWish.Chat:
                return false;
            case EWish.Wander:
                return false;
            default:
                 return items.Any(x => x.Type == type);
        }
    }

    public void AddItem(WishAssistantItem item)
    {
        items.Add(item);
        item.transform.SetParent(controller.GetBoneTransform(HumanBodyBones.LeftHand));
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    public void RemoveItem(WishAssistantItem item)
    {
        items.Remove(item);
    }
}