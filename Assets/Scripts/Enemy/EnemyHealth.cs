using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] 
    private float maxHealth = 100f;

    [SerializeField]
    private HealthBar healthBar;

    [SerializeField]
    private SoundEffect hitSound;

    private float currentHealth;

    // Callbacks that the controller subscribes to, to allow us to pass the message forward
    event Action<float> onDamage;
    event Action onDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(((int)maxHealth));
    }

    public bool IsDead()
    {
        return currentHealth <= 0f;
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

        if(hitSound)
            hitSound.Play();
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);
        onDamage?.Invoke(amount);

        if (currentHealth <= 0f)
            onDeath?.Invoke();
    }
}
