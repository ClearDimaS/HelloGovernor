using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private BoxCollider sizeCollider;

    public Vector3 mapSize => sizeCollider.size;
    public Vector3 mapCenter => center.position;
}
