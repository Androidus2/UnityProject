using UnityEngine;

[CreateAssetMenu(fileName = "New Mission Object", menuName = "Scripts/InventorySystem/Items/Equipment")]
public class EquipmentObject : ItemObject
{
    [SerializeField]
    private float attackBonus;

    [SerializeField]
    private float defenseBonus;

    //these can be altered when we get to the attack mechanism as a whole

    public void Awake()
    {
        type = ItemType.Equipment;
    }

   
}