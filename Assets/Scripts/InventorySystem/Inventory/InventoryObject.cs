using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Scripts/InventorySystem/Inventory")]
public class InventoryObject : ScriptableObject
{
    private List<InventorySlot> items = new List<InventorySlot>();
    public void AddItem(ItemObject item)
    {
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
}


