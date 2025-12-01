using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayInventory : MonoBehaviour
{

    [SerializeField]
    private InventoryObject inventory;

    [SerializeField]
    private int xStart;

    [SerializeField]
    private int yStart;

    [SerializeField]
    private int xSpaceBetweenItems; //space between item collumns
    
    [SerializeField]
    private int ySpaceBetweenItems;
    
    [SerializeField]
    private int numberOfColumns;

    [SerializeField]
    private float itemScaleDuration = 0.15f;


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
       

        for(int i = 0; i < inventory.GetItems().Count; i++) //loading up the inventory on game start
        {
            var obj = Instantiate(inventory.GetItems(i).GetItem().GetIcon(), Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.GetItems(i).GetItem().name;
            itemsDisplayed.Add(inventory.GetItems(i), obj);

        }
    }

    public Vector3 GetPosition(int i) //where to display the item
    {
        return new Vector3(xStart + (xSpaceBetweenItems * (i % numberOfColumns)), yStart + (-ySpaceBetweenItems * (i/numberOfColumns)), 0f);
    }

    void CreateNewItemEntry(InventorySlot slot, int index)
    {
        var obj = Instantiate(slot.GetItem().GetIcon(), Vector3.zero, Quaternion.identity, transform);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.localPosition = GetPosition(index);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.GetItem().name;

        itemsDisplayed.Add(slot, obj);

        // Add tween to make it smoothly pop up
        rect.localScale = Vector3.zero;
        rect.DOScale(Vector3.one, itemScaleDuration).SetEase(Ease.OutBack, 1.3f);
    }


    public void UpdateDisplay()// TO DO - instead of checking every frame of the inventory has been updated,
                                    //have the 'owner' of the inventory call to update the display to add or remove an item
    {
        for (int i = 0; i < inventory.GetItems().Count; i++)
        {
            if (!itemsDisplayed.ContainsKey(inventory.GetItems(i)))
            {
                CreateNewItemEntry(inventory.GetItems(i), i);
            }
        }
    }

    
}
