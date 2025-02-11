using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class Interactor : CulledBehaviour
{
    [Serializable]
    public class ItemsData
    {
        public ItemsWishGranter granter;
        public Transform[] places;
        public Transform root;
        public HumanBodyBones bone;
        [ReadOnly] public Transform boneTransform;
        [ReadOnly] public List<WishAssistantItem> interactables;
    }

    [SerializeField] private List<ItemsData> itemDatas;
    private Animator controller;

    private Dictionary<Type, ItemsData> datasDict = new ();
    private Transform rootsParent;

    protected override void OnAwake()
    {
        base.OnAwake();
        controller = GetComponentInChildren<Animator>();
        datasDict = itemDatas.ToDictionary(x => x.granter.GetType(), x => x);
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (visible)
        {
            foreach (var data in itemDatas)
            {
                var hasItems = data.interactables.Count > 0;
                if (data.root.gameObject.activeSelf != hasItems)
                {
                    data.root.gameObject.SetActive(hasItems);
                }
            }
        }
    }

    protected override void OnLateUpdate(bool visible)
    {
        base.OnLateUpdate(visible);
        if (visible)
        {
            if (controller != null)
            {
                foreach (var data in itemDatas)
                {
                    if(data.boneTransform == null)
                    {
                        data.boneTransform = controller.GetBoneTransform(data.bone);
                    }

                    data.root.position = data.boneTransform.position;
                }
            }
        }
    }

    public bool HasItem(ItemsWishGranter granter)
    {
        return datasDict[granter.GetType()].interactables.Count > 0;
    }

    public void AddItem(WishAssistantItem item)
    {
        var data = datasDict[item.Type.GetType()];
        var place = data.places[data.interactables.Count];
        datasDict[item.Type.GetType()].interactables.Add(item);

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

    public WishAssistantItem RemoveItem(ItemsWishGranter granter)
    {
        var data = datasDict[granter.GetType()];
        var item = data.interactables[0];
        data.interactables.RemoveAt(0);
        return item;
    }

    public bool HasMorePlaceFor(ItemsWishGranter granter)
    {
        return datasDict[granter.GetType()].interactables.Count < datasDict[granter.GetType()].places.Length;
    }

    public bool HasAnyIKItem()
    {
        foreach (var dataPair in datasDict)
        {
            foreach (var interactable in dataPair.Value.interactables)
            {
                if (interactable.IsIK)
                {
                    return true;
                }
            }
        }

        return false;
    }
}