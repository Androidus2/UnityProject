using UnityEngine;

[CreateAssetMenu(fileName = "New Health Object", menuName = "Scripts/InventorySystem/Items/Health")]
public class HealthObject : ItemObject
{
    [SerializeField]
    private int restoreHealthValue; //positive or negative value, if we want 'fake' health items

    public void Awake()
    {
        type = ItemType.Health;
    }

}