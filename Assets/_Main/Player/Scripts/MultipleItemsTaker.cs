using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MultipleItemsTaker : MonoBehaviour, IItemTaker, IWishAssistant
{
    [Serializable]
    public class ItemsData
    {
        public EInteractable type;
        public Transform[] places;
        public GameObject root;
        [ReadOnly] public List<WishAssistantItem> interactables;
    }

    [SerializeField] private List<ItemsData> itemDatas;

    private Dictionary<EInteractable, ItemsData> datasDict = new ();

    private void Awake()
    {
        datasDict = itemDatas.ToDictionary(x => x.type, x => x);
    }

    private void Update()
    {
        foreach (var data in itemDatas)
        {
            var hasItems = data.interactables.Count > 0;
            if (data.root.activeSelf != hasItems)
            {
                data.root.SetActive(hasItems);
            }
        }
    }

    public bool CanAddItems(EInteractable type)
    {
        var data = datasDict[type];
        return data.places.Length > data .interactables.Count;
    }

    public void AddItem(WishAssistantItem item)
    {
        var data = datasDict[item.Type.ToInteractable()];
        var place = data.places[data.interactables.Count];
        
        item.transform.SetParent(place);
        item.transform.DOLocalMove(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
        item.transform.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
        data.interactables.Add(item);
    }

    public bool CanServeType(EWish type)
    {
        return datasDict.ContainsKey(type.ToInteractable());
    }
}