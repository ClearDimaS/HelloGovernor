using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IGridPlaceable
{
    public Transform Root { get; }
    public Vector3 GetWorldSize();
}

public class GridPlacer<T> : MonoBehaviour where T : IGridPlaceable
{
    [SerializeField] private BoxCollider sizeCollider;
    [SerializeField] private Vector3Int counts;
    [SerializeField] private Transform placesParent;
    [SerializeField] private List<BoxCollider> places;
    [SerializeField] private List<Transform> placesTransforms;
    
    protected List<T> placedObjects = new ();

    protected Vector3 placeSize;
    public int Count => placedObjects.Count;
    public int MaxPlaces => placesTransforms.Count;

    private void Awake()
    {
        placesTransforms = places.Select(x => x.transform).ToList();
        placeSize = places[0].size;
        for (var i = 0; i < places.Count; i++)
        {
            var place = places[i];
            Destroy(place);
        }
    }

    [Button]
    private void CreatePlaces()
    {
        places.Clear();
        var s = sizeCollider.size;
        var placeSize = new Vector3(s.x / counts.x, s.y / counts.y, s.z / counts.z);
        for (int x = 0; x < counts.x; x++)
        {
            for (int z = 0; z < counts.z; z++)
            {
                for (int y = 0; y < counts.y; y++)
                {
                    var newPlaceGO = new GameObject($"{places.Count}");
                    var place = newPlaceGO.AddComponent<BoxCollider>();
                    place.size = placeSize;
                    place.transform.SetParent(placesParent);
                    place.transform.localPosition = Vector3.Scale(placeSize, new Vector3(x, y, z)) + placeSize / 2f - new Vector3(sizeCollider.size.x, sizeCollider.size.y, sizeCollider.size.z) / 2f;
                    places.Add(place);
                }
            }
        }
    }
    
    public bool CanAddOneMore()
    {
        return placedObjects.Count < placesTransforms.Count;
    }
    
    public void Add(T item, bool immediate = false)
    {
        var place = placesTransforms[placedObjects.Count];
        item.Root.SetParent(place.transform, true);
        if (immediate)
        {
            item.Root.localPosition = Vector3.zero;
            item.Root.localRotation = Quaternion.identity;
        }
        else
        {
            item.Root.DOLocalMove(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
            item.Root.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);   
        }

        var itemSize = item.GetWorldSize();
        var placeSizeWorld = place.transform.TransformVector(placeSize);
        item.Root.localScale = new Vector3(placeSizeWorld.x /itemSize.x, placeSizeWorld.y /itemSize.y, placeSizeWorld.z /itemSize.z) ;
        placedObjects.Add(item);
    }


    public T Remove()
    {
        var retVal = placedObjects[placedObjects.Count - 1];
        placedObjects.RemoveAt(placedObjects.Count - 1);
        retVal.Root.DOKill();
        retVal.Root.SetParent(null, true);
        return retVal;
    }
}