using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class GenericItemsPool
{
    private DiContainer container;
    protected GenericCitizenItem[] prefabs;

    private Transform parent;
    private Queue<GenericCitizenItem> spawnedDict = new ();

    public GenericItemsPool(DiContainer container, params GenericCitizenItem[] prefabs)
    {
        this.prefabs = prefabs;
        this.container = container;
    }

    public GenericCitizenItem GetElement()
    {
        if (spawnedDict == null)
        {
            spawnedDict = new();
        }
        
        if (spawnedDict.Count == 0)
        {
            var list = prefabs;
            var prefab = list[UnityEngine.Random.Range(0, list.Length)];
            spawnedDict.Enqueue(container.InstantiatePrefabForComponent<GenericCitizenItem>(prefab));
        }

        var element = spawnedDict.Dequeue();
        if (parent == null)
        {
            parent = new GameObject("[GenericItemsPool]").transform;
        }
        element.transform.SetParent(parent);
        element.OnReset();
        element.gameObject.SetActive(true);
        element.SetPool(this);
        return element;
    }

    public virtual void Pool(GenericCitizenItem element)
    {
        element.transform.DOKill();
        spawnedDict.Enqueue(element);
        element.OnPool();
        element.gameObject.SetActive(false);
        if (parent == null)
        {
            parent = new GameObject("[GenericItemsPool]").transform;
        }
        element.transform.SetParent(parent);
    }
}