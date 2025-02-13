using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

[Serializable]
public class ItemPlacesData
{
    public ItemConfigData data;
    public Transform[] places;
    public Vector3 rootLocalPlace;
}
public class Interactor : CulledBehaviour
{
    [SerializeField] private ItemPlacesData[] places;
    [HideInInspector] public List<CitizenItem> interactables = new ();

    private Dictionary<ItemConfigData, ItemPlacesData> placesDict;

    protected override void OnAwake()
    {
        base.OnAwake();
        placesDict = places.ToDictionary(x => x.data, x => x);
    }

    public bool HasAnyItem()
    {
        return interactables.Count > 0;
    }

    public void AddItem(CitizenItem item)
    {
        if (interactables.Count > 0 && interactables[0].GetData().key != item.GetData().key)
        {
            var count = interactables.Count;
            for (int i = 0; i < count; i++)
            {
                RemoveItem();
                item.PoolPlease();
            }
        }
        interactables.Add(item);
    }

    public CitizenItem RemoveItem()
    {
        if (interactables.Count > 0)
        {
            var items = interactables;
            var item = items[^1];
            items.RemoveAt(items.Count-1);
            return item;
        }

        return null;
    }

    public bool HasMorePlaceFor(ItemsWishGranter granter)
    {
        return interactables.Count == 0 || interactables.Count < placesDict[interactables[0].GetData()].places.Length;
    }

    public bool HasItemOfType(string key)
    {
        return interactables.Count > 0 && interactables[0].GetData().key == key;
    }

    public Transform GetPlace(int i)
    {
        var data = placesDict[interactables[0].GetData()];
        return data.places[i % data.places.Length];
    }

    public Vector3 GetItemsRootLocalPlace()
    {
        if (interactables.Count == 0)
        {
            return Vector3.zero;
        }
        else
        {
            return placesDict[interactables[0].GetData()].rootLocalPlace;
        }
    }
}