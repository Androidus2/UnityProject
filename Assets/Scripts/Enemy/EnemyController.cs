using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

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
    private int stealValue;
    private InventoryObject playerInventory; //for stealing coins

    [Header("References")]
    [SerializeField] 
    private EnemyHeadLook headLook;
    [SerializeField]
    private Animator anim;
    [SerializeField] // This could be extended to have a loot table to choose from
    private Transform lootDrop;
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private Transform deathEffect;
    [SerializeField]
    private SoundEffect investigateSound;
    [SerializeField]
    private SoundEffect chaseSound;

    private Transform player;
    private EnemyMovement movement;
    private EnemyVision vision;
    private EnemyAttack attack;
    private EnemyHealth health;
    private CapsuleCollider capsuleCollider;

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

    private float lastInvestigateSoundTimer = 0f;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        attack = GetComponent<EnemyAttack>();
        vision = GetComponent<EnemyVision>();
        health = GetComponent<EnemyHealth>();

        capsuleCollider = GetComponent<CapsuleCollider>();


        
        if (playerInventory == null)
        {
            playerInventory = Resources.Load<InventoryObject>("Inventory/PlayerInventory");
        }

        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory ScriptableObject NOT FOUND");
        }
    
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
        lastInvestigateSoundTimer += Time.deltaTime;
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
        if (movement)
        {
            movement.Stop();
            anim.SetFloat("Speed", 0f);
        }
    }


    private void UpdatePatrol()
    {
        anim.SetFloat("Speed", 3f);
        movement.Patrol();
        // If we see the player while investigating, start chasing him
        if (vision.CanSeePlayer())
        {
            state = EnemyState.Chase;
            chaseSound.Play();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        // If the player is very close, start chasing him
        if (dist <= followRange)
        {
            state = EnemyState.Chase;
            chaseSound.Play();
            return;
        }
    }


    private void UpdateChase()
    {
        anim.SetFloat("Speed", 5f);
        float dist = Vector3.Distance(transform.position, player.position);

        // We always know when the player is super close to us, even if he is behind us
        if (dist <= followRange && vision.HasLineOfSight())
        {
            lastSeenPlayerPos = player.position;
            movement.Chase(player);

            if (dist <= attackRange)
            {
                attack.EnterAttack();
                state = EnemyState.Attack;
                anim.SetFloat("Speed", 0f);
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
                anim.SetFloat("Speed", 0f);
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
        if (dist > attackRange || !vision.HasLineOfSight())
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
        anim.SetFloat("Speed", 3f);
        // If we see the player while investigating, start chasing him
        if (vision.CanSeePlayer())
        {
            ExitInvestigate();
            state = EnemyState.Chase;
            chaseSound.Play();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        // If the player is very close, start chasing him
        if (dist <= followRange)
        {
            ExitInvestigate();
            state = EnemyState.Chase;
            chaseSound.Play();
            return;
        }

        // If we are still not at the investigation point, keep going
        if (!movement.ReachedInvestigatePoint(investigateTolerance))
            return;

        // If we reached the investigation point, look around in hopes we can gain LOS to player
        if (investigateTimer == 0f)
        {
            headLook.StartLooking();
            anim.SetBool("Looking Around", true);
        }
        investigateTimer += Time.deltaTime;
        anim.SetFloat("Speed", 0f);

        // If we looked around for too long, give up and go back to patrol
        if (investigateTimer >= investigateDuration)
        {
            ExitInvestigate();
            state = EnemyState.Patrol;
        }
    }

    private void TryPlayInvestigateSound()
    {
        if (lastInvestigateSoundTimer >= 1f)
        {
            lastInvestigateSoundTimer = 0f;
            investigateSound.Play();
        }
    }

    private void EnterInvestigate(Vector3 point)
    {

        investigateTimer = 0f;
        lostSightTimer = 0f;
        lastSeenPlayerPos = point;

        TryPlayInvestigateSound();
        movement.SetInvestigatePoint(point);

        state = EnemyState.Investigate;
    }

    private void ExitInvestigate()
    {
        headLook.StopLooking();
        anim.SetBool("Looking Around", false);

        movement.Stop();
    }

    private void OnDamaged(float amt)
    {
        anim.SetTrigger("Hit");
        if (isPassive)
            return;

        lastSeenPlayerPos = player.position;

        state = EnemyState.Chase;
    }

    private void OnDeath()
    {
        if (movement)
            movement.Stop();

        // Disable the collider and the agent to make it no longer interact with the world
        if (capsuleCollider)
            capsuleCollider.enabled = false;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent)
            agent.enabled = false;

        if (lootDrop)
        {
            Transform loot = Instantiate(lootDrop);
            Vector3 startPos = transform.position;

            // Start hidden and lower
            loot.localScale = Vector3.zero;
            loot.localPosition = new Vector3(startPos.x, startPos.y - 3f, startPos.z);

            Sequence lootSeq = DOTween.Sequence();

            lootSeq.AppendInterval(6f)
               .Append(loot.DOScale(1.1f, 0.35f).SetEase(Ease.OutBack))
               .Join(loot.DOLocalMoveY(startPos.y, 0.35f).SetEase(Ease.OutCubic))
               .Append(loot.DOScale(1f, 0.1f).SetEase(Ease.OutQuad))
               .SetUpdate(true);
        }
        if (deathEffect)
        {
            Transform onDeathEffect = Instantiate(deathEffect);
            onDeathEffect.position = transform.position - new Vector3(0, 1, 0);
            onDeathEffect.gameObject.SetActive(false);

            Sequence seq = DOTween.Sequence()
                .SetUpdate(true)
                .AppendInterval(3.5f)
                .AppendCallback(() => onDeathEffect.gameObject.SetActive(true));

            Destroy(onDeathEffect.gameObject, 10f);
        }

        // TODO: Make this system better instead of manually disabling these components
        if (anim)
        {
            anim.applyRootMotion = true;
            anim.SetTrigger("Die");
        }
        else // If there is no animator yet, just disable the enemy so the player knows it died
            gameObject.SetActive(false);

        // If there is an animator, disable the individual components
        if (headLook)
            headLook.enabled = false;
        if (movement)
            movement.enabled = false;
        if(vision)
            vision.enabled = false;
        if(attack)
            attack.enabled = false;
        if(health)
            health.enabled = false;
        if (canvas)
            canvas.SetActive(false);
        // Also disable the controller
        enabled = false;

        //karma system
        Karma.GetInstance().AddKarmaKill(1);
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
        stealValue = Random.Range(3, 7);
        playerInventory.AddCoins(stealValue);
        Debug.Log(gameObject.name + " was stolen from!");

        //karma system
        Karma.GetInstance().AddKarmaSteal(1);
    }

    public Transform GetPlayerTransform()
    {
        return player.transform;
    }

    public Animator GetAnimator()
    {
        return anim;
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
