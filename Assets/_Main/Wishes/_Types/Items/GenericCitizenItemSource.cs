using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GenericCitizenItemSource : CulledBehaviour
{
    [Inject] private DiContainer container;
    [Inject] private PlayerController player;

    [SerializeField] private float radius = 1f;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image takeProgressImage;
    [field: SerializeField] public Transform TakePlace { get; private set; }
    [field: SerializeField] public Transform IdlePlace { get; private set; }
    public int TakesCount { get; protected set; }

    private HashSet<IItemTaker> takers = new ();
    private List<IItemTaker> giveItemToTakersTMP = new ();
    private Dictionary<IItemTaker, float> takerTimers = new ();
    private GenericItemsPool pool;
    
    [SerializeField] protected Sprite icon;
    [SerializeField] protected GenericCitizenItem[] prefabs;
    [SerializeField] private float takeDuration;
    
    protected override void OnAwake()
    {
        base.OnAwake();
        var itemsGranter = GetComponentInParent<ItemsWishGranter>(true);
        if (itemsGranter != null)
        {
            icon = itemsGranter.GetItemIcon();
            prefabs = itemsGranter.GetPrefabs();
            takeDuration = itemsGranter.GetTakeItemDuration();
        }
        else
        {
            var orderGranter = GetComponentInParent<OrderWishGranter>(true);
            if (orderGranter != null)
            {
                takeDuration = orderGranter.GetTakeItemDuration();   
            }
        }
    }

    private void Start()
    {
        iconImage.sprite = icon;
        pool = new GenericItemsPool(container, prefabs);
        takerTimers[player] = 0;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        var progress = 0f;

        if (visible)
        {
            var diffToPlayer = TakePlace.position - player.transform.position;
            diffToPlayer.y = 0f;
            if (diffToPlayer.magnitude < radius && player.CanAddItems())
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
            var duration = takeDuration;
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
            if (taker is PlayerController)
            {
                TakesCount++;
            }
            if (!taker.CanAddItems())
            {
                RemoveTaker(taker);
            }
        }
        
        giveItemToTakersTMP.Clear();

        takeProgressImage.fillAmount = progress;
    }

    private GenericCitizenItem TakeItem()
    {
        var element = pool.GetElement();
        element.transform.position = transform.position;
        return element;
    }

    public void AddTaker(IItemTaker taker)
    {
        if (!takers.Contains(taker) && taker.CanAddItems())
        {
            takers.Add(taker);
            takerTimers[taker] = 0f;   
        }
    }

    public void RemoveTaker(IItemTaker taker)
    {
        takers.Remove(taker);
        takerTimers[taker] = 0f;   
    }

    public bool HasPrefab(GenericCitizenItem prefab)
    {
        foreach (var p in prefabs)
        {
            if (p == prefab)
            {
                return true;
            }  
        }

        return false;
    }
}