using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyVision : MonoBehaviour
{
    [SerializeField] private Transform enemyEyes;
    [SerializeField] private float viewRange = 20f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private LayerMask obstacleMask;

    EnemyController controller;
    Transform player;

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    private void Start()
    {
        player = controller.GetPlayerTransform();
    }

    public bool IsInRange(float rangeOverride = -1f)
    {
        float range = (rangeOverride > 0f) ? rangeOverride : viewRange;
        return Vector3.Distance(enemyEyes.position, player.position) <= range;
    }

    public bool IsInFOV()
    {
        Vector3 dir = (player.position - enemyEyes.position).normalized;
        float angle = Vector3.Angle(enemyEyes.forward, dir);
        return angle <= viewAngle * 0.5f;
    }

    public bool HasLineOfSight()
    {
        Vector3 dir = (player.position - enemyEyes.position).normalized;
        float dist = Vector3.Distance(enemyEyes.position, player.position);

        if (Physics.Raycast(enemyEyes.position, dir, dist, obstacleMask))
            return false;

        return true;
    }

    public bool CanSeePlayer()
    {
        if (!IsInRange()) return false;
        if (!IsInFOV()) return false;
        if (!HasLineOfSight()) return false;

        return true;
    }
}
