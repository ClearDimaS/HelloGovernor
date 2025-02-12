using System.Collections.Generic;
using DG.Tweening;
using Zenject;

public class GenericItemsPool
{
    private DiContainer container;
    protected GenericCitizenItem[] prefabs;

    private Queue<GenericCitizenItem> spawnedDict = new ();

    public GenericItemsPool(GenericCitizenItem[] prefabs, DiContainer container)
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
    }
}