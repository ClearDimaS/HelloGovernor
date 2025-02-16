using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class IcecreamGranter : OperatableGranter
{
    [Inject] protected DiContainer container;

    [SerializeField] protected Transform giveItemFrom;
    [SerializeField] protected GenericCitizenItem[] itemPrefabs;
    protected GenericItemsPool pool;
    [SerializeField] protected Transform[] itemPlaces;
    private Queue<GenericCitizenItem> itemsQueue = new ();

    private void Start()
    {
        pool = new GenericItemsPool(itemPrefabs, container);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (operatablePlace.GetFillCount > itemsQueue.Count)
        {
            var element = pool.GetElement();
            element.SetPool(pool);
            element.transform.position = itemPlaces[itemsQueue.Count%itemPlaces.Length].position;
            itemsQueue.Enqueue(element);
        }
    }

    protected override void OnLeave(CitizenController citizen)
    {
        base.OnLeave(citizen);
        GenericCitizenItem item;
        if (itemsQueue.Count > 0)
        {
            item = itemsQueue.Dequeue();
        }
        else
        {
            var element = pool.GetElement();
            element.SetPool(pool);
            item = element;
        }
        item.transform.position = giveItemFrom.position;
        citizen.AddItem(item);
    }
}