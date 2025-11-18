using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float health = 100f;

    // Callbacks that the controller subscribes to, to allow us to pass the message forward
    event Action<float> onDamage;
    event Action onDeath;

    public bool IsDead()
    {
        return health <= 0f;
    }

    public void SubscribeToDamage(Action<float> callback) 
    { 
        onDamage += callback; 
    }
    public void UnsubscribeFromDamage(Action<float> callback) 
    { 
        onDamage -= callback; 
    }

    public void SubscribeToDeath(Action callback)
    {
        onDeath += callback;
    }

    public void UnsubscribeFromDeath(Action callback)
    {
        onDeath -= callback;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead()) return;

        health -= amount;
        onDamage?.Invoke(amount);

        if (health <= 0f)
            onDeath?.Invoke();
    }
}
