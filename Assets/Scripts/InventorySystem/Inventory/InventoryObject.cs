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

    // --- MODIFICARE NOUA: Functia pentru a sterge un item specific ---
    public bool RemoveItem(ItemObject itemToRemove)
    {
        // Cautam prin toate sloturile
        for (int i = 0; i < items.Count; i++)
        {
            // Daca gasim itemul cautat
            if (items[i].GetItem() == itemToRemove)
            {
                items.RemoveAt(i); // Il stergem din lista
                return true; // Returnam true (am reusit sa stergem)
            }
        }
        return false; // Nu am gasit itemul, deci nu am sters nimic
    }
}