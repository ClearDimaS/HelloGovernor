using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public interface IResetable
{
    public void OnReset();
    
    public void OnPool();
}
public class MonoPool<T> : MonoBehaviour where T : MonoBehaviour, IResetable
{
    [Inject] private DiContainer container;
    
    [SerializeField] private T prefab;

    private Queue<T> spawned = new ();

    public T GetElement()
    {
        if (spawned.Count == 0)
        {
            spawned.Enqueue(container.InstantiatePrefabForComponent<T>(prefab, transform));
        }

        var element = spawned.Dequeue();
        element.OnReset();
        element.gameObject.SetActive(true);
        return element;
    }

    public virtual void Pool(T element)
    {
        element.transform.DOKill();
        spawned.Enqueue(element);
        element.OnPool();
        element.gameObject.SetActive(false);
    }
}

public class PlainPool<T> where T : IResetable
{
    [Inject] private DiContainer container;

    private Queue<T> spawned = new ();

    public T GetElement()
    {
        if (spawned.Count == 0)
        {
            spawned.Enqueue(container.Instantiate<T>());
        }

        var element = spawned.Dequeue();
        element.OnReset();
        return element;
    }

    public virtual void Pool(T element)
    {
        spawned.Enqueue(element);
        element.OnPool();
    }
}

public class MonoPoolCollection<T, U> : MonoBehaviour where T : MonoBehaviour, IResetable, IKey<U>
{
    [Inject] private DiContainer container;
    
    [SerializeField] protected T[] prefabs;

    private Dictionary<U,  Queue<T>> spawnedDict = new ();
    private Dictionary<U,  T> prefabsDict = new ();

    private void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        foreach (var prefab in prefabs)
        {
            prefabsDict[prefab.Key] = prefab;
        }
    }

    public T GetElement(U key)
    {
        if (!spawnedDict.ContainsKey(key))
        {
            spawnedDict[key] = new Queue<T>();
        }
        
        var spawned = spawnedDict[key];
        if (spawned.Count == 0)
        {
            var prefab = prefabsDict[key];
            spawned.Enqueue(container.InstantiatePrefabForComponent<T>(prefab));
        }

        var element = spawned.Dequeue();
        element.OnReset();
        element.gameObject.SetActive(true);
        return element;
    }

    public virtual void Pool(T element)
    {
        if (!spawnedDict.ContainsKey(element.Key))
        {
            spawnedDict[element.Key] = new Queue<T>();
        }
        element.transform.DOKill();
        spawnedDict[element.Key].Enqueue(element);
        element.OnPool();
        element.gameObject.SetActive(false);
    }
}

public class MonoPoolCollectionMultiple<T, U> : MonoBehaviour where T : MonoBehaviour, IResetable, IKey<U>
{
    [Inject] private DiContainer container;
    
    [SerializeField] protected T[] prefabs;

    private Dictionary<U,  Queue<T>> spawnedDict = new ();
    private Dictionary<U,  List<T>> prefabsDict = new ();

    private void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        foreach (var prefab in prefabs)
        {
            if (prefab == null)
            {
                continue;
            }
            if (!prefabsDict.ContainsKey(prefab.Key))
            {
                prefabsDict[prefab.Key] = new List<T>();
            }
            prefabsDict[prefab.Key].Add(prefab);
        }
    }

    public T GetElement(U key)
    {
        if (!spawnedDict.ContainsKey(key))
        {
            spawnedDict[key] = new Queue<T>();
        }
        
        var spawned = spawnedDict[key];
        if (spawned.Count == 0)
        {
            var list = prefabsDict[key];
            var prefab = list[Random.Range(0, list.Count)];
            spawned.Enqueue(container.InstantiatePrefabForComponent<T>(prefab));
        }

        var element = spawned.Dequeue();
        element.OnReset();
        element.gameObject.SetActive(true);
        return element;
    }

    public virtual void Pool(T element)
    {
        if (!spawnedDict.ContainsKey(element.Key))
        {
            spawnedDict[element.Key] = new Queue<T>();
        }
        element.transform.DOKill();
        spawnedDict[element.Key].Enqueue(element);
        element.OnPool();
        element.gameObject.SetActive(false);
    }
}
