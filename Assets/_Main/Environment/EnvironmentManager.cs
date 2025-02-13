using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private BoxCollider sizeCollider;

    public Vector3 mapSize => sizeCollider.size;
    public Vector3 mapCenter => center.position;

    public Vector3 GetRandomUnlockedPosition(float safetyRadius)
    {
        var pos = mapCenter + new Vector3(
            Random.Range(-mapSize.x / 2f, mapSize.x / 2f),
            0,
            Random.Range(-mapSize.z / 2f, mapSize.z / 2f));
        NavMesh.SamplePosition(pos, out var hit, 100f, -1);
        pos = hit.position;
        return pos;
    }
}
