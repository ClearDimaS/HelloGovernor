using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class Interactor : CulledBehaviour
{
    [HideInInspector] public List<CitizenItem> interactables = new ();
    private Animator animator;
    private Dictionary<string, ItemsPlacesData> placesDict = new ();
    
    protected override void OnAwake()
    {
        base.OnAwake();
        animator = GetComponentInChildren<Animator>();
    }

    public bool HasAnyItem()
    {
        return interactables.Count > 0;
    }

    public void AddItem(CitizenItem item)
    {
        var data = item.GetData();
        if (!placesDict.ContainsKey(data.key))
        {
            placesDict[data.key] = item.CreatePlaces(animator);
        }

        interactables.Add(item);
        
        var place = placesDict[data.key].GetPlace(interactables.Count);
        
        item.transform.DOKill();
        item.transform.SetParent(place);
        var middle = (item.transform.position + place.position) / 2f;
        middle.y = place.position.y + 1f;
        item.transform.DOMove(middle, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            item.transform.DOLocalMove(Vector3.zero, 0.15f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                var startScale = item.transform.localScale;
                item.transform.DOScale(startScale * 1.3f, 0.2f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    item.transform.DOScale(startScale, 0.2f).SetEase(Ease.InCubic);
                });
            }); 
        });
        item.transform.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
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
        return interactables.Count < 1;
    }

    public bool HasItemOfType(string key)
    {
        return interactables.Count > 0 && interactables[0].GetData().key == key;
    }
}