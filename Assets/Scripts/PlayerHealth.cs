using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int maxHealth;

    private int currentHealth;

    [SerializeField]
    private HealthBar healthBar;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private MonoBehaviour[] playerComponents;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        // We don't want these commands to be active, but we're keeping them in if we need to test something
        // TestCommands();
    }

    void TestCommands()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
     
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        healthBar.SetHealth(currentHealth);
    }

    void Die()
    {
        Debug.Log("Player is dead.");
        animator.applyRootMotion = true;
        animator.SetTrigger("Die");

        // TODO: Make a better way of disabling the player's inputs on death
        foreach(var player in playerComponents) 
            player.enabled = false;

        // TODO: Add a death screen instead of directly resetting the scene
        StartCoroutine(WaitBeforeRestartingScene());
    }

    IEnumerator WaitBeforeRestartingScene()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Heal(int amount)
    {
        if (currentHealth >= maxHealth)
        {
            Debug.Log("Player health already full!");
            return;
        }

        currentHealth += amount;
      
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);

        Debug.Log("Player healed " + amount + " health. Current health: " + currentHealth);
    }
   
}
