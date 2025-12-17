using UnityEngine;

[CreateAssetMenu(fileName = "New Health Object", menuName = "Scripts/InventorySystem/Items/Money")]


public class MoneyObject : ItemObject
{
    [SerializeField]
    private int coinValue; //value of the money item

    public void Awake()
    {
        type = ItemType.Money;
    }

    public int GetValue()
    {
        return coinValue;
    }


}
