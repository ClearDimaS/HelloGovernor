using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

[Serializable]
public class ItemPlacesData
{
    public ItemConfigData data;
    public GameObject rootGO;
    public Vector3 rootLocalPlace;
    public Transform[] places;
}
public class Interactor : CulledBehaviour
{
    [SerializeField] private ItemPlacesData[] places;
    [HideInInspector] public List<CitizenItem> interactables = new ();
    [SerializeField] private PlayerController playerController;
    
    private Dictionary<ItemConfigData, ItemPlacesData> placesDict;

    protected override void OnAwake()
    {
        base.OnAwake();
        placesDict = places.ToDictionary(x => x.data, x => x);
        RefreshRootsVisibility();
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
                var oldItem = RemoveItem();
                if (oldItem != null)
                {
                    oldItem.PoolPlease();
                }
            }
        }
        interactables.Add(item);
        RefreshRootsVisibility();
    }

    public CitizenItem RemoveItem()
    {
        if (interactables.Count > 0)
        {
            var items = interactables;
            var item = items[^1];
            items.RemoveAt(items.Count-1);
            RefreshRootsVisibility();
            return item;
        }
        
        return null;
    }

    private void RefreshRootsVisibility()
    {
        var visible = interactables.Count > 0;
        if (visible)
        {
            foreach (var placesData in places)
            {
                if (placesData.rootGO != null && !placesData.rootGO.activeSelf && placesData.data == interactables[0].GetData())
                {
                    placesData.rootGO.SetActive(true);
                }
            }
        }
        else
        {
            foreach (var placesData in places)
            {
                if (placesData.rootGO != null && placesData.rootGO.activeSelf)
                {
                    placesData.rootGO.SetActive(false);
                }
            }
        }
    }

    public CitizenItem RemoveItem(CitizenItem item)
    {
        if (interactables.Count > 0)
        {
            var items = interactables;
            items.Remove(item);
            return item;
        }

        return null;
    }

    private int GetMaxItemsCount()
    {
        if (playerController == null)
        {
            if (interactables.Count > 0)
            {
                return placesDict[interactables[0].GetData()].places.Length;
            }

            return 0;
        }

        return playerController.Capacity;
    }
    
    public bool HasMorePlaceFor()
    {
        return interactables.Count == 0 || interactables.Count < GetMaxItemsCount();
    }

    public bool HasItemOfType(string key)
    {
        return interactables.Count > 0 && interactables[0].GetData().key == key;
    }

    public Transform GetPlace(int i)
    {
        var data = placesDict[interactables[0].GetData()];
        if (i >= data.places.Length && playerController != null)
        {
            var lastPlace = data.places[^1];
            var list = data.places.ToList();
            var newLastPlaceGO = new GameObject("NewLastPlaceGO");
            newLastPlaceGO.transform.SetParent(lastPlace.parent);
            newLastPlaceGO.transform.localPosition = lastPlace.localPosition;
            newLastPlaceGO.transform.localRotation = lastPlace.localRotation;
            newLastPlaceGO.transform.localScale = lastPlace.localScale;
            var newLastPlace = newLastPlaceGO.transform;
            var prevLastPlace = data.places[data.places.Length - 2];
            var lpDiff = lastPlace.localPosition.y - prevLastPlace.localPosition.y;
            if (lpDiff > 0.01f)
            {
                newLastPlace.localPosition = lastPlace.localPosition + Vector3.up * lpDiff;
            }
            else
            {
                var randPlace = data.places[Random.Range(0, data.places.Length)];
                newLastPlace.localPosition = randPlace.localPosition + Vector3.up * 0.2f;
            }
            list.Add(newLastPlace);
            data.places = list.ToArray();
        }
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

    public int GetCurrentMaxPlaces()
    {
        return GetMaxItemsCount();
    }

    public void ClearItems()
    {
        foreach (var interactable in interactables)
        {
            interactable.PoolPlease();
        }
        interactables.Clear();
    }
}