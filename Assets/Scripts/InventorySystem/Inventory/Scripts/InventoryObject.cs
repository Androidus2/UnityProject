using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Scripts/InventorySystem/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> items = new List<InventorySlot>();
    public void AddItem(ItemObject item)
    {
        items.Add(new InventorySlot(item));
    }
    
}

[SerializeField]
public class InventorySlot
{
    public ItemObject item;
    //here, im choosing to not have 'slots'; repeats of the same objects (i dont expect there to be many) will take each 1 slot
    //if we decide to add this later on, it starts with public int amount;
    public InventorySlot(ItemObject _item)
    {
        item = _item;
    }
}
