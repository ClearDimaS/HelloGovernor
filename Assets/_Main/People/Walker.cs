using System;
using UnityEngine;
using UnityEngine.AI;

public class Walker : CulledBehaviour
{
    [SerializeField] private float stopDistance = 0.15f;
    [SerializeField] private float speed;
    [SerializeField] private NavMeshAgent agent;

    public float Speed => speed;
    private Vector3 target;
    private Vector3 hitTarget;
    public bool IsMoving => !isFinished;
    protected bool isFinished = true;
    private NavMeshHit hit;

    private event Action reachTargetEvent;
    
    private void Start()
    {
        agent.speed = speed;
        agent.stoppingDistance = stopDistance;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (HasReached())
        {
            SetFinished();   
        }
    }

    private void SetFinished()
    {
        isFinished = true;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        if (reachTargetEvent != null)
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
            agent.Warp(hit.position);
        }
    }

    public bool IsMovingToTarget(Vector3 target)
    {
        return IsMoving && IsSameTarget(target, this.target);
    }

    public void MoveToTarget(Vector3 target, Action onReachTarget, float radiusOverride = -1f)
    {
        if (NavMesh.SamplePosition(target, out hit, Mathf.Infinity, NavMesh.AllAreas))
        {
            hitTarget = hit.position;
            reachTargetEvent = onReachTarget;
            if (HasReached(radiusOverride))
            {
                SetFinished();
                return;
            }

            agent.isStopped = false;
            isFinished = false;
            this.target = target;
            agent.SetDestination(hit.position);   
        }
    }

    protected bool HasReached(float radiusOverride = -1f)
    {
        var stop = stopDistance;
        if (radiusOverride > 0f)
        {
            stop = radiusOverride;
        }
        return stop > (transform.position - hitTarget).magnitude;
    }
    
    private bool IsSameTarget(Vector3 target1, Vector3 target2)
    {
        return Mathf.Approximately(target1.x, target2.x) && Mathf.Approximately(target1.y, target2.y) &&
               Mathf.Approximately(target1.z, target2.z);
    }

}