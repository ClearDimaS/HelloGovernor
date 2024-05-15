using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public interface IItemTaker
{
    public bool CanAddItems(EInteractable type);
    public void AddItem(WishAssistantItem takeItem);
}

public class WishAssistantItemSource : MonoBehaviour
{
    [Inject] private WishItemsConfig config;
    [Inject] private WishAssistantItemsPool pool;

    [SerializeField] private Image iconImage;
    [SerializeField] private Image takeProgressImage;
    [field: SerializeField] public Transform TakePlace { get; private set; }
    private WishGranter wishGranter;

    private HashSet<IItemTaker> takers = new ();
    private List<IItemTaker> giveItemToTakersTMP = new ();
    private Dictionary<IItemTaker, float> takerTimers = new ();

    private void Awake()
    {
        wishGranter = GetComponentInParent<WishGranter>();
        iconImage.sprite = config.GetIcon(wishGranter.Type);
    }

    private void Update()
    {
        var progress = 0f;

        foreach (var taker in takers)
        {
            takerTimers[taker] += Time.deltaTime;
            var timer = takerTimers[taker];
            var duration = config.GetTakeDuration(wishGranter.Type);
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
        }
        
        giveItemToTakersTMP.Clear();

        takeProgressImage.fillAmount = progress;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.attachedRigidbody == null)
        {
            return;
        }

        var otherRB = other.attachedRigidbody;
        if (!otherRB.TryGetComponent<IItemTaker>(out var player))
        {
            return;
        }
        AddTaker(player);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger || other.attachedRigidbody == null)
        {
            return;
        }

        var otherRB = other.attachedRigidbody;
        if (!otherRB.TryGetComponent<IItemTaker>(out var player))
        {
            return;
        }
        RemoveTaker(player);
    }

    private WishAssistantItem TakeItem()
    {
        return pool.GetElement(wishGranter.Type);
    }

    public void AddTaker(IItemTaker taker)
    {
        if (!takers.Contains(taker))
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