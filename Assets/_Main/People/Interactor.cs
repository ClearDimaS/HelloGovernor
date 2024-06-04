using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public enum EInteractable
{
    None,
    Drink,
    Flowers,
    IceCream
}
public class Interactor : MonoBehaviour
{
    [Serializable]
    public class ItemsData
    {
        public EInteractable type;
        public Transform[] places;
        public Transform root;
        public HumanBodyBones bone;
        [ReadOnly] public Transform boneTransform;
        [ReadOnly] public List<WishAssistantItem> interactables;
    }

    [SerializeField] private List<ItemsData> itemDatas;
    private Animator controller;

    private Dictionary<EInteractable, ItemsData> datasDict = new ();
    private Transform rootsParent;
    
    private void Awake()
    {
        controller = GetComponentInChildren<Animator>();
        datasDict = itemDatas.ToDictionary(x => x.type, x => x);
    }
    
    private void Update()
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

    private void LateUpdate()
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
    
    public bool HasItem(EWish type)
    {
        switch (type)
        {
            case EWish.Chat:
                return false;
            case EWish.Wander:
                return false;
            default:
                return datasDict[type.ToInteractable()].interactables.Count > 0;
        }
    }

    public void AddItem(WishAssistantItem item)
    {
        var data = datasDict[item.Type.ToInteractable()];
        var place = data.places[data.interactables.Count];
        datasDict[item.Type.ToInteractable()].interactables.Add(item);

        item.transform.DOKill();
        item.transform.SetParent(place);
        item.transform.DOLocalMove(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
        item.transform.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
    }

    public WishAssistantItem RemoveItem(EWish type)
    {
        var data = datasDict[type.ToInteractable()];
        var item = data.interactables[0];
        data.interactables.RemoveAt(0);
        return item;
    }

    public bool HasMorePlaceFor(EInteractable type)
    {
        return datasDict[type].interactables.Count < datasDict[type].places.Length;
    }

    public bool HasAnyIKItem()
    {
        return HasItem(EWish.Drinks) || HasItem(EWish.IceCream);
    }
}