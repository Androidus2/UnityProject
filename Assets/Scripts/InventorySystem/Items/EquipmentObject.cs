using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armour
    
}

[CreateAssetMenu(fileName = "New Mission Object", menuName = "Scripts/InventorySystem/Items/Equipment")]
public class EquipmentObject : ItemObject
{
    [SerializeField]
    private EquipmentType equipmentType;

    [SerializeField]
    private float bonusValue; //attack bonus for weapon, defense bonus for armour

    public void Awake()
    {
        type = ItemType.Equipment;
    }

    public override bool Use()
    {
        // inventory on click logic will call this function
        if(this.equipmentType is EquipmentType.Armour)
            {
            PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.SetDefenseBonus(bonusValue);
            }
        }
        else if(this.equipmentType is EquipmentType.Weapon)
        {
            PlayerAttack playerAttack = FindFirstObjectByType<PlayerAttack>();
            if (playerAttack != null)
            {
                playerAttack.SetAttackBonus(bonusValue);
            }
        }

        Debug.Log($"Equipment Item: equipped for {bonusValue} {equipmentType} bonus" );
        return true; 
    }

    public EquipmentType GetEquipmentType()
    {
        return equipmentType;
    }

    public float GetBonusValue()
    {
        return bonusValue;
    }


}