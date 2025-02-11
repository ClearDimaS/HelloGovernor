using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class WishAssistantItemsPool
{
    private DiContainer container;
    protected WishAssistantItem[] prefabs;

    private Queue<WishAssistantItem> spawnedDict = new ();

    public WishAssistantItemsPool(WishAssistantItem[] prefabs, DiContainer container)
    {
        this.prefabs = prefabs;
        this.container = container;
    }

    public WishAssistantItem GetElement()
    {
        if (spawnedDict == null)
        {
            spawnedDict = new();
        }
        
        if (spawnedDict.Count == 0)
        {
            var list = prefabs;
            var prefab = list[Random.Range(0, list.Length)];
            spawnedDict.Enqueue(container.InstantiatePrefabForComponent<WishAssistantItem>(prefab));
        }

        var element = spawnedDict.Dequeue();
        element.OnReset();
        element.gameObject.SetActive(true);
        return element;
    }

    public virtual void Pool(WishAssistantItem element)
    {
        element.transform.DOKill();
        spawnedDict.Enqueue(element);
        element.OnPool();
        element.gameObject.SetActive(false);
    }
}