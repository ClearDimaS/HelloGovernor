using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class FenceBuilder : MonoBehaviour
{
    [Inject] private UpgradablePricesManager upgradableManager;
    
    [SerializeField] private GameObject fencePrefab;
    [SerializeField] private Vector3 fenceSize = new Vector3(1, 1, 1);

    private int activeFenceIndex;
    private List<GameObject> spawnedFenceParts = new();
    private int lastBought = 0;

    private void Update()
    {
        if (lastBought != upgradableManager.BoughtCount)
        {
            lastBought = upgradableManager.BoughtCount;
            BuildFence();
        }
    }

    [Button]
    private void BuildFence()
    {
        var boughtBuildings = upgradableManager.GetBoughtBuildings();
        var bounds = new Bounds();
        bounds.center = boughtBuildings[0].Center;
        bounds.size = Vector3.zero;
        foreach (var building in boughtBuildings)
        {
            foreach (var minMaxData in building.GetCorners())
            {
                bounds.Encapsulate(minMaxData.min + building.Center);
                bounds.Encapsulate(minMaxData.max + building.Center);
            }
        }

        HideFence();

        var min = bounds.min;
        var max = bounds.max;
        var rot = Quaternion.LookRotation(Vector3.right, Vector3.up);
        for (float x = min.x; x < max.x; x+=fenceSize.x)
        {
            var pos = new Vector3(x, 0, max.z);
            PlaceFence(pos, rot);
        }
        rot = Quaternion.LookRotation(Vector3.back, Vector3.up);
        for (float z = max.z; z > min.z; z-=fenceSize.x)
        {
            var pos = new Vector3(max.x, 0, z);
            PlaceFence(pos, rot);
        }
        rot = Quaternion.LookRotation(Vector3.left, Vector3.up);
        for (float x = max.x; x > min.x; x-=fenceSize.x)
        {
            var pos = new Vector3(x, 0, min.z);
            PlaceFence(pos, rot);
        }
        rot = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        for (float z = min.z; z < max.z; z+=fenceSize.x)
        {
            var pos = new Vector3(min.x, 0, z);
            PlaceFence(pos, rot);
        }
    }

    private void HideFence()
    {
        foreach (var part in  spawnedFenceParts)
        {
            part.gameObject.SetActive(false);
        }

        activeFenceIndex = 0;
    }

    private void PlaceFence(Vector3 pos, Quaternion rot)
    {
        if (spawnedFenceParts.Count <= activeFenceIndex)
        {
            var newFence = Instantiate(fencePrefab, transform);
            spawnedFenceParts.Add(newFence);
        }
        else
        {
            spawnedFenceParts[activeFenceIndex].gameObject.SetActive(true);
        }
        spawnedFenceParts[activeFenceIndex].transform.position = pos;
        spawnedFenceParts[activeFenceIndex].transform.rotation = rot;
        activeFenceIndex++;
    }
}
