using System;
using UnityEngine;
using Zenject;

public class IcecreamGranter : OperatableGranter
{
    [Inject] protected DiContainer container;

    [SerializeField] protected Transform giveItemFrom;
    [SerializeField] protected GenericCitizenItem[] itemPrefabs;
    protected GenericItemsPool pool;

    private void Start()
    {
        pool = new GenericItemsPool(itemPrefabs, container);
    }

    protected override void OnLeave(CitizenController citizen)
    {
        base.OnLeave(citizen);
        var element = pool.GetElement();
        element.SetPool(pool);
        element.transform.position = giveItemFrom.position;
        citizen.AddItem(element);
    }
}