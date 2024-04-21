using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwiperCyclicFacade : MonoBehaviour
{
    [SerializeField] private SimpleElementSwiper swiper;
    [SerializeField] private int activeCount = 5;
    
    private GameObject[] prefabs;
    private int centerIndex;
    private int forceLayer = -1;

    private Dictionary<int, GameObject> spawnedGOs;

    public ElementsSwiper Swiper => swiper;
    public int ElementIndex => swiper.CurElementNumber;

    public void Initialize(GameObject[] prefabs, int centerIndex)
    {
        this.prefabs = prefabs;
        this.centerIndex = centerIndex;
        spawnedGOs = new Dictionary<int, GameObject>(prefabs.Length);

        for (int i = 0; i < prefabs.Length; i++)
        {
            var root = swiper.GetElement(i);
            var spawned = Instantiate(prefabs[i], root);
            if (forceLayer != -1)
            {
                spawned.SetGameLayerRecursive(forceLayer);
            }
            spawnedGOs[i] = spawned;
        }
        swiper.MoveTo(this.centerIndex);
    }
    
    public void MoveElements(int diff, bool immediate)
    {
        swiper.MoveElements(diff, immediate);
    }

    public void SetLayer(int nameToLayer)
    {
        forceLayer = nameToLayer;
    }

    public void MouseDown()
    {
        swiper.OnMouseDown();
    }

    public void Drag()
    {
        swiper.OnMouseDrag();
    }

    public void MouseUp()
    {
        swiper.OnMouseUp();
    }
}