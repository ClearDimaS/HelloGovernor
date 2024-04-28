using System;
using UnityEngine;
using UnityEngine.AI;

public class Walker : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private NavMeshAgent agent;

    public float Speed => speed;
    private Vector3 target;
    private Vector3 hitTarget;
    public bool IsMoving => agent.hasPath && agent.velocity.sqrMagnitude != 0f;
    private NavMeshHit hit;

    private event Action reachTargetEvent;
    
    private void Start()
    {
        agent.speed = speed;
    }

    private void Update()
    {
        if (!IsMoving && reachTargetEvent != null)
        {
            var tmp = reachTargetEvent;
            reachTargetEvent = null;
            tmp.Invoke();
        }
    }

    public void Place(Vector3 target)
    {
        if (NavMesh.SamplePosition(target, out hit, Mathf.Infinity, NavMesh.AllAreas)) 
        {
            transform.position = hit.position;
        }
    }

    public bool IsMovingToTarget(Vector3 target)
    {
        return IsMoving && IsSameTarget(target, this.target);
    }

    public void MoveToTarget(Vector3 target, Action onReachTarget)
    {
        if (NavMesh.SamplePosition(target, out hit, Mathf.Infinity, NavMesh.AllAreas))
        {
            reachTargetEvent = onReachTarget;
            this.target = target;
            hitTarget = hit.position;
            agent.SetDestination(hit.position);   
        }
    }

    private bool IsSameTarget(Vector3 target1, Vector3 target2)
    {
        return Mathf.Approximately(target1.x, target2.x) && Mathf.Approximately(target1.y, target2.y) &&
               Mathf.Approximately(target1.z, target2.z);
    }

}