using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private BoxCollider patrolArea;

    private NavMeshAgent agent;
    public Vector3 GetPatrolAreaCenter()
    { 
        return patrolArea.bounds.center; 
    }

    public Vector3 GetPatrolAreaSize()
    { 
        return patrolArea.bounds.size; 
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.SetDestination(GenerateRandomPatrolPoint());
        }
    }

    private Vector3 GenerateRandomPatrolPoint()
    {
        Bounds b = patrolArea.bounds;

        Vector3 random = new Vector3(
            Random.Range(b.min.x, b.max.x),
            b.center.y,
            Random.Range(b.min.z, b.max.z)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(random, out hit, 2.5f, NavMesh.AllAreas))
            return hit.position;

        // If we were unable to generate a target point, return the current position so the generation will be retried next frame
        return transform.position;
    }

    public void SetInvestigatePoint(Vector3 point)
    {
        agent.SetDestination(point);
    }

    public bool ReachedInvestigatePoint(float tolerance)
    {
        return !agent.pathPending && agent.remainingDistance <= tolerance;
    }

    public void Chase(Transform target)
    {
        if (target == null)
            Debug.LogError("Enemy " + gameObject.name + " is trying to chase null target!");
        agent.SetDestination(target.position);
    }

    public void Stop()
    {
        agent.ResetPath();
    }
}
