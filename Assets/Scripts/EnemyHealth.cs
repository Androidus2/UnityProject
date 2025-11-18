using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    float health;

    public bool IsDead()
    {
        return health <= 0;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead())
            return;
        health -= damage;

        if (health < 0)
            Die();
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}
