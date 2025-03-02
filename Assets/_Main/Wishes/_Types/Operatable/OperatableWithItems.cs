using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class OperatableWithItems : OperatableGranter
{
    [Inject] protected DiContainer container;

    [SerializeField] protected Transform giveItemFrom;
    [SerializeField] protected GenericCitizenItem[] itemPrefabs;
    protected GenericItemsPool pool;
    [SerializeField] protected Transform[] itemPlaces;
    private Stack<GenericCitizenItem> itemsQueue = new ();

    private void Start()
    {
        pool = new GenericItemsPool(container, itemPrefabs);
    }

    protected override void OnLeave(CitizenController citizen)
    {
        base.OnLeave(citizen);
        GenericCitizenItem item;
        if (itemsQueue.Count > 0)
        {
            item = itemsQueue.Pop();
            item.transform.DOKill();
        }
        else
        {
            var element = pool.GetElement();
            item = element;
            item.transform.position = giveItemFrom.position;
        }
        citizen.AddItem(item);
    }

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return base.CanAddProgress(citizen) && itemsQueue.Count > 0;
    }
    
    public void AddItem()
    {
        var element = pool.GetElement();
        element.transform.position = operatablePlace.fillPlace.GetPlace().position + Vector3.up;
        element.transform.DOMove(itemPlaces[itemsQueue.Count % itemPlaces.Length].position, 0.3f)
            .SetEase(Ease.InOutCubic);
        itemsQueue.Push(element);
    }

    public int GetItemsCount()
    {
        return itemsQueue.Count;
    }
}