using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DisplayInventory : MonoBehaviour
{
    public InventoryObject inventory;

    public int XSTART;
    public int YSTART;

    public int XSPACE_BETWEEN_ITEMS; //space between item collumns
    public int YSPACE_BETWEEN_ITEMS;
    public int NUMBER_OF_COLUMNS;


    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();
    void Start()
    {
        CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplay();
    }

    public void CreateDisplay()
    {
       

        for(int i = 0; i < inventory.items.Count; i++) //loading up the inventory on game start
        {
            var obj = Instantiate(inventory.items[i].item.GetIcon(), Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.items[i].item.name;
            itemsDisplayed.Add(inventory.items[i], obj);

        }
    }

    public Vector3 GetPosition(int i) //where to display the item
    {
        return new Vector3(XSTART + (XSPACE_BETWEEN_ITEMS * (i % NUMBER_OF_COLUMNS)), YSTART + (-YSPACE_BETWEEN_ITEMS * (i/NUMBER_OF_COLUMNS)), 0f);
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (!itemsDisplayed.ContainsKey(inventory.items[i]))
            {
                var obj = Instantiate(inventory.items[i].item.GetIcon(), Vector3.zero, Quaternion.identity, transform) as GameObject;
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.items[i].item.name;
                itemsDisplayed.Add(inventory.items[i], obj);
            }
        }
    }

    
}
