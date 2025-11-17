using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackAngle = 60f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int attackDamage = 15;

    EnemyController controller;
    Transform player;

    float cooldownTimer = 0f;

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    private void Start()
    {
        player = controller.GetPlayerTransform();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
    }

    public void EnterAttack()
    {
        // Don't do the attack instantly, we imagine the enemy having a wind up movement before actually hitting
        if (cooldownTimer > attackCooldown * 0.8f)
            cooldownTimer = attackCooldown * 0.8f;
    }

    public bool IsReady()
    {
        return cooldownTimer >= attackCooldown;
    }

    public bool IsPlayerInFront()
    {
        Vector3 toPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, toPlayer);
        float distance = Vector3.Distance(transform.position, player.position);

        return angle <= attackAngle * 0.5f && distance <= attackRange;
    }

    public bool TryAttack()
    {
        if (!IsReady()) return false;

        cooldownTimer = 0f;

        if (IsPlayerInFront())
        {
            if (player.TryGetComponent<PlayerHealth>(out var hp))
            {
                hp.TakeDamage(attackDamage);
            }

            // Since we don't have very good visual cues for now, leave a log to make sure everything works as it should
            Debug.Log("Enemy hit the player.");
            return true;
        }

        // Since we don't have very good visual cues for now, leave a log to make sure everything works as it should
        Debug.Log("Enemy missed the player.");
        return false;
    }
}
