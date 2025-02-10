using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public interface IItemTaker : IRootProvider
{
    public bool CanAddItems(ItemsWishGranter granter);
    public void AddItem(WishAssistantItem takeItem);
}

public class WishAssistantItemSource : CulledBehaviour
{
    [Inject] private WishAssistantItemsPool pool;
    [Inject] private PlayerController player;

    [SerializeField] private float radius = 1f;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image takeProgressImage;
    [field: SerializeField] public Transform TakePlace { get; private set; }
    private ItemsWishGranter wishGranter;

    private HashSet<IItemTaker> takers = new ();
    private List<IItemTaker> giveItemToTakersTMP = new ();
    private Dictionary<IItemTaker, float> takerTimers = new ();

    protected override void OnAwake()
    {
        base.OnAwake();
        wishGranter = GetComponentInParent<ItemsWishGranter>();
        iconImage.sprite = wishGranter.GetItemIcon();
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        var progress = 0f;

        if (visible)
        {
            var diffToPlayer = TakePlace.position - player.transform.position;
            diffToPlayer.y = 0f;
            if (diffToPlayer.magnitude < radius)
            {
                takers.Add(player);
            }
            else
            {
                takers.Remove(player);
            }
        }

        foreach (var taker in takers)
        {
            takerTimers[taker] += Time.deltaTime;
            var timer = takerTimers[taker];
            var duration = wishGranter.GetTakeItemDuration();
            if (timer > duration)
            {
                giveItemToTakersTMP.Add(taker);
            }

            progress = timer / duration;
        }
        
        foreach (var taker in giveItemToTakersTMP)
        {
            takerTimers[taker] = 0f;
            taker.AddItem(TakeItem());
            if (!taker.CanAddItems(wishGranter))
            {
                RemoveTaker(taker);
            }
        }
        
        giveItemToTakersTMP.Clear();

        takeProgressImage.fillAmount = progress;
    }

    private WishAssistantItem TakeItem()
    {
        var element = pool.GetElement(wishGranter);
        element.transform.position = transform.position;
        return element;
    }

    public void AddTaker(IItemTaker taker)
    {
        if (!takers.Contains(taker) && taker.CanAddItems(wishGranter))
        {
            takers.Add(taker);
            takerTimers[taker] = 0f;   
        }
    }

    public void RemoveTaker(IItemTaker taker)
    {
        takers.Remove(taker);
        takerTimers.Remove(taker);
    }
}