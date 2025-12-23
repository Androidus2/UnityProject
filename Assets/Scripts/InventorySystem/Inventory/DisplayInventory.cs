using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour
{

    //154 x  84 y   -68y  equipped items

    private InventoryObject inventory;

    [SerializeField]
    private InventoryObject playerInventory; //to allow chest inventory to add to player inventory

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


    public void Awake() //for chest automatic attribution
    { 
        if (playerInventory == null) 
        {
            playerInventory = Resources.Load<InventoryObject>("Inventory/PlayerInventory"); 
        }
        
        if (playerInventory == null) 
        {
            Debug.LogError("PlayerInventory ScriptableObject NOT FOUND"); 
        }
    }

    public void SetInventoryObject(InventoryObject inv)
    {
        // Set the inventory object to display - can be called from other scripts
        inventory = inv;
        RefreshDisplay();
    }


    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();
    void RefreshDisplay()
    {
        // Clear old UI
        foreach (var obj in itemsDisplayed.Values)
        {
            Destroy(obj);
        }
        itemsDisplayed.Clear();

        if (inventory == null) return;

        switch (inventory.GetInventoryType())
        {
            case InventoryType.Player:
                CreateDisplayPlayer();
                break;

            case InventoryType.Chest:
                CreateDisplayChest();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory == null) return;
        UpdateDisplay();
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

        obj.GetComponent<Button>().onClick.AddListener(() => {
            Debug.Log("Clicked on " + inventory.GetItems(index).GetItem().name);
        });

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


    public void CreateDisplayPlayer() 
    { 
    
        //display the coin count
        Vector3 coinPosition = new Vector3(435, -230, 0f); //fixed position for coin display, down right corner
        GameObject coinPrefab = Resources.Load<GameObject>("Items/CoinDisplay");
        var coinObj = Instantiate(coinPrefab, Vector3.zero, Quaternion.identity, transform);
        coinObj.GetComponent<RectTransform>().localPosition = coinPosition;
        //clean previous text
        coinObj.GetComponentInChildren<TextMeshProUGUI>().text = "";
        coinObj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.GetCoinCount().ToString();
        itemsDisplayed.Add(new InventorySlot(null), coinObj); //using a dummy InventorySlot to hold the coin display so it can be cleared later


        for (int i = 0; i < inventory.GetItems().Count; i++) //loading up the inventory on game start
        {
            int index = i; // capture the current value of i for the closure
            var item = inventory.GetItems(index); //cache the item for use in the listener
            var obj = Instantiate(inventory.GetItems(i).GetItem().GetIcon(), Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.GetItems(i).GetItem().name;

            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("Clicked on " + inventory.GetItems(index).GetItem().name);
                //dropdown menu for use/equip/sell could go here
                //for now, its just use for medicine - need to decide on equipment mechanics

                //dropdown menu activation
                Transform panelTransform = obj.transform.Find("DropdownUse");
                panelTransform.gameObject.SetActive(!panelTransform.gameObject.activeSelf);

                //add listener for the item
                //drop / delete - all of them
                //health items - use (then delete if used)
                //equipment - equip (not implemented yet) / auto switch with equipped 
                //mission items - no action? will get called to be used from other scripts

                Button useButton = panelTransform.GetComponent<Button>();
                useButton.onClick.RemoveAllListeners(); //clear previous listeners to avoid stacking
                useButton.onClick.AddListener(() =>
                {
                    Debug.Log("Used " + inventory.GetItems(index).GetItem().name);
                    var item = inventory.GetItems(index);

                    bool okToDelete = item.GetItem().Use();
                    panelTransform.gameObject.SetActive(false);

                    if (okToDelete)
                    {
                        inventory.GetItems().RemoveAt(index);
                        RefreshDisplay();
                    }

                });
            });
            itemsDisplayed.Add(inventory.GetItems(i), obj);
        }

     }

    public void CreateDisplayChest() 
    {
        for (int i = 0; i < inventory.GetItems().Count; i++) //loading up the inventory on game start
        {
            int index = i; // capture the current value of i for the closure
            var item = inventory.GetItems(index); //cache the item for use in the listener
            var obj = Instantiate(inventory.GetItems(i).GetItem().GetIcon(), Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.GetItems(i).GetItem().name;

            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("Clicked on " + inventory.GetItems(index).GetItem().name);

                //dropdown menu for 'take'
                Transform panelTransform = obj.transform.Find("DropdownUse");
                panelTransform.gameObject.SetActive(!panelTransform.gameObject.activeSelf);

                //chest inventory - add to player inventory
                panelTransform.GetComponentInChildren<TextMeshProUGUI>().text = "Take";
                Button takeButton = panelTransform.GetComponent<Button>();
                takeButton.onClick.RemoveAllListeners(); //clear previous listeners to avoid stacking
                takeButton.onClick.AddListener(() =>
                {
                    Debug.Log("Took " + item.GetItem().name);

                    if (playerInventory.AddItem(item.GetItem()))
                    {
                        inventory.GetItems().RemoveAt(index);
                        RefreshDisplay();
                    }
                    else
                    {
                        Debug.Log("Not enough space in player inventory to take " + item.GetItem().name);
                    }
                    panelTransform.gameObject.SetActive(false);

                });
            });

            itemsDisplayed.Add(inventory.GetItems(i), obj);
        }

    }

}
