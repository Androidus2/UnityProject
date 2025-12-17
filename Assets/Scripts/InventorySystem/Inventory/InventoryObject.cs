using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Scripts/InventorySystem/Inventory")]
public class InventoryObject : ScriptableObject
{
    private int coinCount = 0;

    private List<InventorySlot> items = new List<InventorySlot>();
    public void AddItem(ItemObject item)
    {
        //check if its money item
        if (item is MoneyObject moneyItem)
        {
            AddCoins(moneyItem.GetValue());
            return;
        }
        //else add to inventory
        items.Add(new InventorySlot(item));
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
}


