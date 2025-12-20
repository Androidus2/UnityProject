using UnityEngine;

[CreateAssetMenu(fileName = "New Health Object", menuName = "Scripts/InventorySystem/Items/Health")]
public class HealthObject : ItemObject
{
    [SerializeField]
    private int restoreHealthValue; //positive or negative value, if we want 'fake' health items

    private PlayerHealth playerHealth;

    public void Awake()
    {
        type = ItemType.Health;
    }

    public override bool Use()
    {
        // inventory on click logic will call this function
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        playerHealth.Heal(restoreHealthValue);
        Debug.Log($"Restored {restoreHealthValue} health.");
        return true; //indicate that the item was used and can be removed from inventory
    }

}