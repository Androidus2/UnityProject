using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Investigate,
        Attack
    }

    [Header("Enemy information")]
    [SerializeField]
    private EnemyState state = EnemyState.Idle;
    [SerializeField]
    private bool isPassive = true;

    [SerializeField]
    private bool isStealable = true;

    [Header("References")]
    [SerializeField] 
    private EnemyHeadLook headLook;

    private Transform player;
    private EnemyMovement movement;
    private EnemyVision vision;
    private EnemyAttack attack;
    private EnemyHealth health;

    [Header("Ranges & Timing")]
    [SerializeField] private float followRange = 3f;       // The range where we always know the player's position
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float lostSightDelay = 1f;

    [Header("Investigate Settings")]
    [SerializeField] private float investigateDuration = 4f;
    [SerializeField] private float investigateTolerance = 0.6f;

    private float lostSightTimer = 0f;
    private float investigateTimer = 0f;
    private Vector3 lastSeenPlayerPos;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        attack = GetComponent<EnemyAttack>();
        vision = GetComponent<EnemyVision>();
        health = GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        if (health != null)
        {
            health.SubscribeToDamage(OnDamaged);
            health.SubscribeToDeath(OnDeath);
        }
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.UnsubscribeFromDamage(OnDamaged);
            health.UnsubscribeFromDeath(OnDeath);
        }
    }


    private void Update()
    {
        switch (state)
        {
            case EnemyState.Idle: UpdateIdle(); break;
            case EnemyState.Patrol: UpdatePatrol(); break;
            case EnemyState.Chase: UpdateChase(); break;
            case EnemyState.Investigate: UpdateInvestigate(); break;
            case EnemyState.Attack: UpdateAttack(); break;
        }
    }


    private void UpdateIdle()
    {
        if(movement)
            movement.Stop();
    }


    private void UpdatePatrol()
    {
        movement.Patrol();
    }


    private void UpdateChase()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        // We always know when the player is super close to us, even if he is behind us
        if (dist <= followRange)
        {
            lastSeenPlayerPos = player.position;
            movement.Chase(player);

            if (dist <= attackRange)
            {
                attack.EnterAttack();
                state = EnemyState.Attack;
                movement.Stop();
            }

            return;
        }

        // If we can see the player, chase him
        if (vision.CanSeePlayer())
        {
            lostSightTimer = 0f;
            lastSeenPlayerPos = player.position;

            if (dist <= attackRange)
            {
                attack.EnterAttack();
                state = EnemyState.Attack;
                movement.Stop();
                return;
            }

            movement.Chase(player);
            return;
        }

        lostSightTimer += Time.deltaTime;

        // If too much time has passed and we didn't regain sight of player, investigate his last known position
        if (lostSightTimer >= lostSightDelay)
        {
            EnterInvestigate(lastSeenPlayerPos);
            return;
        }

        // If we lost sight, but not enough time has passed, continue going to the last known position of the player, while staying in the chase state
        movement.SetInvestigatePoint(lastSeenPlayerPos);
    }

    private void UpdateAttack()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        // If the player has moved too far, start chasing
        if (dist > attackRange)
        {
            state = EnemyState.Chase;
            return;
        }

        // Rotate torso toward the player during attack
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, target, 8f * Time.deltaTime
            );
        }

        // If the player is super close or we have LOS, we can try to attack
        if (vision.CanSeePlayer() || vision.HasClearMeleeLine(followRange))
        {
            lastSeenPlayerPos = player.position;
            if (attack.IsReady())
                attack.TryAttack();
        }
        else
        {
            // If we lost LOS, investigate the last known position
            EnterInvestigate(lastSeenPlayerPos);
        }
    }

    private void UpdateInvestigate()
    {
        // If we see the player while investigating, start chasing him
        if (vision.CanSeePlayer())
        {
            ExitInvestigate();
            state = EnemyState.Chase;
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        // If the player is very close, start chasing him
        if (dist <= followRange)
        {
            ExitInvestigate();
            state = EnemyState.Chase;
            return;
        }

        // If we are still not at the investigation point, keep going
        if (!movement.ReachedInvestigatePoint(investigateTolerance))
            return;

        // If we reached the investigation point, look around in hopes we can gain LOS to player
        investigateTimer += Time.deltaTime;

        // If we looked around for too long, give up and go back to patrol
        if (investigateTimer >= investigateDuration)
        {
            ExitInvestigate();
            state = EnemyState.Patrol;
        }
    }

    private void EnterInvestigate(Vector3 point)
    {
        investigateTimer = 0f;
        lostSightTimer = 0f;
        lastSeenPlayerPos = point;

        movement.SetInvestigatePoint(point);

        // If we weren't investigating already, look around too
        if (state != EnemyState.Investigate)
            headLook.StartLooking();

        state = EnemyState.Investigate;
    }

    private void ExitInvestigate()
    {
        headLook.StopLooking();

        movement.Stop();
    }

    private void OnDamaged(float amt)
    {
        if (isPassive)
            return;

        lastSeenPlayerPos = player.position;

        state = EnemyState.Chase;
    }

    private void OnDeath()
    {
        if(movement)
            movement.Stop();
        gameObject.SetActive(false);
    }

    public void HearSound(Vector3 point)
    {
        if (isPassive)
            return;
        // The enemy shouldn't stop attacking or chasing because of a sound
        if (state != EnemyState.Patrol && state != EnemyState.Investigate)
            return;

        // --- Only use X and Z for area check ---
        Vector2 soundXZ = new Vector2(point.x, point.z);

        // Get patrol area definition from movement component
        Vector2 center = new Vector2(movement.GetPatrolAreaCenter().x, movement.GetPatrolAreaCenter().z);
        Vector2 halfSize = new Vector2(movement.GetPatrolAreaSize().x * 0.5f,
                                       movement.GetPatrolAreaSize().z * 0.5f);

        bool inside =
            soundXZ.x >= center.x - halfSize.x &&
            soundXZ.x <= center.x + halfSize.x &&
            soundXZ.y >= center.y - halfSize.y &&
            soundXZ.y <= center.y + halfSize.y;

        // Ignore sounds outside patrol area
        if (!inside)
            return;

        // If inside, we should investigate
        EnterInvestigate(point);
    }

    public void MarkStolen()
    {
        isStealable = false;
        Debug.Log(gameObject.name + " was stolen from!");
    }

    public Transform GetPlayerTransform()
    {
        return player.transform;
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    public bool GetIsStealable()
    {
        return isStealable;
    }

}
