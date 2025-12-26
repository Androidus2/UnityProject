using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackAngle = 60f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDamage = 15;

    [SerializeField]
    private Transform hitEffect;

    EnemyController controller;
    Transform player;
    Animator anim;

    float cooldownTimer = 0f;

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    private void Start()
    {
        player = controller.GetPlayerTransform();
        anim = controller.GetAnimator();
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

    public void TryAttack()
    {
        anim.SetTrigger("Attack");
        cooldownTimer = 0f;

        Debug.Log("Starting attack!");
        StartCoroutine(WaitBeforeDealingDamage());
    }

    void DoDamageEffect(Vector3 position)
    {
        Transform newHitEffect = Instantiate(hitEffect);
        newHitEffect.position = position;
        Destroy(newHitEffect.gameObject, 3f);
    }

    IEnumerator WaitBeforeDealingDamage()
    {
        yield return new WaitForSeconds(0.5f);
        if (IsPlayerInFront())
        {
            if (player.TryGetComponent<PlayerHealth>(out var hp))
            {
                hp.TakeDamage(attackDamage);
            }
            DoDamageEffect(player.position);
            // Since we don't have very good visual cues for now, leave a log to make sure everything works as it should
            Debug.Log("Enemy hit the player.");
        }

        // Since we don't have very good visual cues for now, leave a log to make sure everything works as it should
        Debug.Log("Enemy missed the player.");
    }
}
