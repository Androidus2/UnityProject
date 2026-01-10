using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    private float currentHealth;

    private float defenseBonus;

    [SerializeField]
    private HealthBar healthBar;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private MonoBehaviour[] playerComponents;

    [SerializeField]
    private Fade fade;

    [SerializeField]
    private Transform deathEffect;

    [SerializeField]
    private SoundEffect hitSound;

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

    public void TakeDamage(float damage)
    {
        // If we are already dead, don't keep taking damage and calling the Die method
        if (currentHealth <= 0)
            return;

        hitSound.Play();
        animator.SetTrigger("Hit");
        currentHealth -= damage - defenseBonus;
     
        Debug.Log("Player took " + (damage - defenseBonus) + " damage. Current health: " + currentHealth);

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

        Transform onDeathEffect = Instantiate(deathEffect);
        onDeathEffect.position = transform.position;
        onDeathEffect.gameObject.SetActive(false);

        Sequence seq = DOTween.Sequence()
            .SetUpdate(true)
            .AppendInterval(2f)
            .AppendCallback(() => onDeathEffect.gameObject.SetActive(true));

        // TODO: Make a better way of disabling the player's inputs on death
        foreach (var player in playerComponents) 
            player.enabled = false;

        // TODO: Add a death screen instead of directly resetting the scene
        StartCoroutine(WaitBeforeRestartingScene());
    }

    IEnumerator WaitBeforeRestartingScene()
    {
        yield return new WaitForSeconds(4f);
        fade.BeginFade(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }

    public void Heal(float amount)
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

    public void SetDefenseBonus(float bonus)
    {
        defenseBonus = bonus;
    }

}
