using System.Collections.Generic;
using UnityEngine;
public enum InventoryType
{
    Player,
    Chest
}

[CreateAssetMenu(fileName = "New Inventory", menuName = "Scripts/InventorySystem/Inventory")]
public class InventoryObject : ScriptableObject
{
    [SerializeField]
    private InventoryType type;

    private int coinCount = 0;

    [SerializeField]
    private int inventorySize = 9;

    private List<InventorySlot> items = new List<InventorySlot>();
    public bool AddItem(ItemObject item)
    {
        //check if its money item
        if (item is MoneyObject moneyItem)
        {
            AddCoins(moneyItem.GetValue());
            return true;
        }
        //else add to inventory 
        if (items.Count >= inventorySize)
        {
            Debug.Log("Inventory Full");
            return false;
        }
        items.Add(new InventorySlot(item));
        return true;
    }

    public bool AddCoinItem(ItemObject item) //for chest inventories
    {
        if (items.Count >= inventorySize)
        {
            Debug.Log("Inventory Full");
            return false;
        }
        items.Add(new InventorySlot(item));
        return true;
    }

    public List<InventorySlot> GetItems()
    {
        return items;
    }

    public InventorySlot GetItems(int i)
    {
        return items[i];
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
    }

    public void ClearInventory()
    {
        items.Clear();
        coinCount = 0;
    }

    public bool RemoveItem(int index)
    {
        this.items.RemoveAt(index);
        return true;
    }

    public InventoryType GetInventoryType()
    {
        return type;
    }
}


