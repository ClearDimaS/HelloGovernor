using System.Collections.Generic;
using UnityEngine;

public interface IGridPlaceable
{
    public Transform Root { get; }
    public Vector3 GetWorldSize();
}

public class GridPlacer<T> : MonoBehaviour where T : IGridPlaceable
{
    private Transform[] places;
    private List<T> placedObjects = new ();

    public int Count => placedObjects.Count;
    
    public bool CanAddOneMore()
    {
        return placedObjects.Count < places.Length;
    }
    
    public void Add(T item)
    {
        item.Root.SetParent(places[placedObjects.Count]);
        placedObjects.Add(item);
    }


    public T Remove()
    {
        return placedObjects[placedObjects.Count - 1];
    }
}