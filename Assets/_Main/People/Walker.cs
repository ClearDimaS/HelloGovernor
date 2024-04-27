using UnityEngine;
using UnityEngine.AI;

public class Walker : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private NavMeshAgent agent;

    public float Speed => speed;
    public bool IsMoving => agent.hasPath && agent.velocity.sqrMagnitude != 0f;
    private NavMeshHit hit;
    
    private void Start()
    {
        agent.speed = speed;
    }
    
    public void Place(Vector3 target)
    {
        if (NavMesh.SamplePosition(target, out hit, Mathf.Infinity, NavMesh.AllAreas)) 
        {
            transform.position = hit.position;
        }
    }

    public void SetWalkTarget(Vector3 target)
    {
        if (NavMesh.SamplePosition(target, out hit, Mathf.Infinity, NavMesh.AllAreas)) 
        {
            agent.SetDestination(hit.position);   
        }
    }

}