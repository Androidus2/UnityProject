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

                //find dropdown menu
                Transform panelTransform = obj.transform.Find("DropdownMenu");

                //add listener for the item
                //health items - use (then delete if used) + drop
                //equipment - equip (not implemented yet) / auto switch with equipped + drop
                //mission items - no action, will get called to be used from other scripts

                if(item.GetItem() is MissionObject)
                { return; } //no action for mission items

                //find first button + make visibile
                Transform firstButtonTransform = panelTransform.Find("FirstButton");
                firstButtonTransform.gameObject.SetActive(!firstButtonTransform.gameObject.activeSelf);

                Button firstButton = firstButtonTransform.GetComponent<Button>();

                //set listener depending on item type

                if(item.GetItem() is HealthObject)
                {
                    AddButtonListener(firstButton, firstButtonTransform, "Use", index);
                }
                else if(item.GetItem() is EquipmentObject)
                {
                    AddButtonListener(firstButton, firstButtonTransform, "Equip", index);
                }

                //set drop button listener
                Transform secondButtonTransform = panelTransform.Find("SecondButton");
                secondButtonTransform.gameObject.SetActive(!secondButtonTransform.gameObject.activeSelf);

                Button secondButton = secondButtonTransform.GetComponent<Button>();

                if(item.GetItem() is HealthObject || item.GetItem() is EquipmentObject)
                {
                    AddButtonListener(secondButton, secondButtonTransform, "Drop", index);
                }



            });
            itemsDisplayed.Add(inventory.GetItems(i), obj);
        }

     }

    public void CreateDisplayChest() 
    {
        for (int i = 0; i < inventory.GetItems().Count; i++) //loading up the inventory on game start
        {
            int index = i; // capture the current value of i for the closure
            var obj = Instantiate(inventory.GetItems(i).GetItem().GetIcon(), Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.GetItems(i).GetItem().name;

            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("Clicked on " + inventory.GetItems(index).GetItem().name);

                //find dropdown menu, make sure its active
                Transform panelTransform = obj.transform.Find("DropdownMenu");

                //get first button + make visibile - take item
                Transform buttonTransform = panelTransform.Find("FirstButton");
                buttonTransform.gameObject.SetActive(!buttonTransform.gameObject.activeSelf);

                //chest inventory - add to player inventory
                
                Button takeButton = buttonTransform.GetComponent<Button>();
                AddButtonListener(takeButton, panelTransform, "Take", index);
            });

            itemsDisplayed.Add(inventory.GetItems(i), obj);
        }

    }

    private void AddButtonListener(Button button, Transform buttonTransform, string action, int index)
    {
        button.onClick.RemoveAllListeners(); //clear previous listeners to avoid stacking
        var item = inventory.GetItems(index); //cache the item 

        //set the text
        buttonTransform.GetComponentInChildren<TextMeshProUGUI>().text = action;

        switch (action)
        {
            case "Use":
                button.onClick.AddListener(() =>
                {
                    Debug.Log("Used " + inventory.GetItems(index).GetItem().name);

                    bool okToDelete = item.GetItem().Use();
                    buttonTransform.gameObject.SetActive(false);

                    if (okToDelete)
                    {
                        inventory.GetItems().RemoveAt(index);
                        RefreshDisplay();
                    }

                });
                break;
            case "Equip":
                button.onClick.AddListener(() =>
                {
                    Debug.Log("Equipped " + inventory.GetItems(index).GetItem().name);

                    //equip logic here
                    buttonTransform.gameObject.SetActive(false);

                    //swap items logic here

                });
                break;
            case "Drop":
                button.onClick.AddListener(() =>
                {
                    Debug.Log("Dropped " + inventory.GetItems(index).GetItem().name);
                    inventory.GetItems().RemoveAt(index);
                    RefreshDisplay();
                    buttonTransform.gameObject.SetActive(false);
                });
                break;
            case "Take":
                button.onClick.AddListener(() =>
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
                    buttonTransform.gameObject.SetActive(false);

                });
                break;
            default:
                Debug.LogWarning("Action " + action + " not recognized.");
                break;
        }
        
    }

}
