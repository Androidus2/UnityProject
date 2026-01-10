using System.Collections.Generic;
using UnityEngine;
public enum InventoryType
{
    Player,
    Chest,
    Vendor
}

public class InventoryObject : MonoBehaviour
{
    [SerializeField]
    private string inventoryName; //for identification, espcially for chests / vendors

    [SerializeField]
    private InventoryType type;

    private int coinCount = 0;

    [SerializeField]
    private int inventorySize = 9;

    [SerializeField]
    private SoundEffect forbiddenSound;

    [SerializeField]
    private SoundEffect pickupSound;

    [SerializeField]
    private SoundEffect coinSound;

    [SerializeField]
    private List<InventorySlot> items = new List<InventorySlot>();
    public bool AddItem(ItemObject item)
    {
        //check if its money item
        if (item is MoneyObject moneyItem)
        {
            if (type == InventoryType.Player)
                coinSound.Play();
            AddCoins(moneyItem.GetValue());
            return true;
        }
        //else add to inventory 
        if (items.Count >= inventorySize)
        {
            if (type == InventoryType.Player)
                forbiddenSound.Play();
            Debug.Log("Inventory Full");
            return false;
        }
        if(type == InventoryType.Player)
            pickupSound.Play();
        items.Add(new InventorySlot(item));
        return true;
    }

    public void Insert(ItemObject item, int index) //for swapping items
    {
        items.RemoveAt(index);
        items.Insert(index, new InventorySlot(item));
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

    public void SetCoinCount(int amount)
    {
        coinCount = amount;
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

    public string GetInventoryName()
    {
        return inventoryName;
    }

    public void SetType(InventoryType inventoryType)
    {
        type = inventoryType;
    }

    public void SetSize(int size)
    {
        inventorySize = size;
    }
}


 