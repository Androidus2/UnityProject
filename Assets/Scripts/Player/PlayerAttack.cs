using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    InputAction attackAction;
    float attackValue;

    [SerializeField]
    float attackDamage;

    private float attackBonus;

    [SerializeField]
    float attackCooldown;

    [SerializeField]
    Animator animator;

    [SerializeField]
    private Transform hitEffect;

    float timeSinceLastAttack;

    List<EnemyHealth> targets;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");

        timeSinceLastAttack = attackCooldown;

        targets = new List<EnemyHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        attackValue = attackAction.ReadValue<float>();
        timeSinceLastAttack += Time.deltaTime;
        if(attackValue > 0f && timeSinceLastAttack >= attackCooldown && PlayerMechanicsUnlocker.Instance.IsMechanicUnlocked("Attacking"))
        {
            BeginAttack();
        }
    }

    void BeginAttack()
    {
        timeSinceLastAttack = 0f;
        animator.SetTrigger("Attack");
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
        if (targets.Count > 0)
        {
            EnemyHealth hitEnemy = targets[0];
            hitEnemy.TakeDamage(attackDamage + attackBonus);
            DoDamageEffect(hitEnemy.transform.position);
            if (hitEnemy.IsDead())
                targets.RemoveAt(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy == null)
            {
                Debug.LogError("Found an enemy without an EnemyHealth component!");
                return;
            }

            targets.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy == null)
            {
                Debug.LogError("Found an enemy without an EnemyHealth component!");
                return;
            }

            targets.Remove(enemy);
        }
    }

    public void SetAttackBonus(float bonus)
    {
        attackBonus = bonus;
    }
}
