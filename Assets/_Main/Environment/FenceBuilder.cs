using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
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
        if (lastBought != upgradableManager.AvailableCount)
        {
            lastBought = upgradableManager.AvailableCount;
            BuildFence();
        }
    }

    [Button]
    private void BuildFence()
    {
        var boughtBuildings = upgradableManager.GetAvailableBuildings();
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
        Vector3 prevPos = new Vector3(0, -99, 0);
        var y = -1f;
        var add = fenceSize.x;
        for (float x = min.x; x < max.x; x+=add)
        {
            var pos = new Vector3(x, y, max.z);
            prevPos = PlaceFence(pos, prevPos, rot);
            y = prevPos.y;
        }
        rot = Quaternion.LookRotation(Vector3.back, Vector3.up);
        prevPos = new Vector3(0, -99, 0);
        for (float z = max.z; z > min.z; z-=add)
        {
            var pos = new Vector3(max.x, y, z);
            prevPos = PlaceFence(pos, prevPos, rot);
            y = prevPos.y;
        }
        rot = Quaternion.LookRotation(Vector3.left, Vector3.up);
        prevPos = new Vector3(0, -99, 0);
        for (float x = max.x; x > min.x; x-=add)
        {
            var pos = new Vector3(x, y, min.z);
            prevPos = PlaceFence(pos, prevPos, rot);
            y = prevPos.y;
        }
        rot = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        prevPos = new Vector3(0, -99, 0);
        for (float z = min.z; z < max.z; z+=add)
        {
            var pos = new Vector3(min.x, y, z);
            prevPos = PlaceFence(pos, prevPos, rot);
            y = prevPos.y;
        }
    }

    private Vector3 PlaceFence(Vector3 pos, Vector3 prevPos, Quaternion rot)
    {
        prevPos = PlaceFenceTile(pos, prevPos, rot);
        return prevPos;
    }

    private void HideFence()
    {
        foreach (var part in  spawnedFenceParts)
        {
            part.gameObject.SetActive(false);
        }

        activeFenceIndex = 0;
    }

    private Vector3 PlaceFenceTile(Vector3 pos, Vector3 prevPos, Quaternion rot)
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

        var isHit = NavMesh.SamplePosition(pos, out var hit, 10f, -1);
        pos.y = hit.position.y;
        if (Mathf.Abs(prevPos.y - hit.position.y) > 0.1f && prevPos.y > -30f && activeFenceIndex > 0)
        {
            var prevRot = Quaternion.LookRotation((pos - prevPos).normalized);
            spawnedFenceParts[activeFenceIndex-1].transform.rotation = prevRot;
        }
        spawnedFenceParts[activeFenceIndex].transform.position = pos;
        spawnedFenceParts[activeFenceIndex].transform.rotation = rot;
        activeFenceIndex++;
        return pos;
    }
}
