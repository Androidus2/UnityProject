using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int maxHealth;

    private int currentHealth;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
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
            Debug.Log("Player is dead.");
        }
        healthBar.SetHealth(currentHealth);
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
    void OnGUI()
    {
        GUI.contentColor = Color.black;
        GUI.Label(new Rect(10, 10, 200, 20), "Health: " + currentHealth + "/" + maxHealth);
    }

}
