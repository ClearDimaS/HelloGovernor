using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

public class CitizenController : MonoBehaviour
{
    [Inject] private EnvironmentManager environment;
    
    [SerializeField] private float speed;
    [SerializeField] private NavMeshAgent agent;

    public float Speed => speed;
    public bool IsMoving => agent.hasPath && agent.velocity.sqrMagnitude != 0f;
    private NavMeshHit hit;
    
    private void Start()
    {
        agent.speed = speed;
    }

    private void Update()
    {
        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
        {
            var pos = GetRandomPos();
            if (NavMesh.SamplePosition(pos, out hit, Mathf.Infinity, NavMesh.AllAreas)) 
            {
                agent.SetDestination(hit.position);   
            }
        }
    }
    
    public void PlaceRandom()
    {
        var pos = GetRandomPos();
        Place(pos);
    }
    
    public void Place(Vector3 target)
    {
        if (NavMesh.SamplePosition(target, out hit, Mathf.Infinity, NavMesh.AllAreas)) 
        {
            transform.position = hit.position;
        }
    }
    
    private Vector3 GetRandomPos()
    {
        var pos = environment.mapCenter + new Vector3(
            Random.Range(-environment.mapSize.x / 2f, environment.mapSize.x / 2f),
            0,
            Random.Range(-environment.mapSize.z / 2f, environment.mapSize.z / 2f));
        return pos;
    }
}
