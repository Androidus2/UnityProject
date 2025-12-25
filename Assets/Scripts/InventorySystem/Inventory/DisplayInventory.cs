using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour
{
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

    private InventorySlot weaponSlot = null;
    private InventorySlot armourSlot = null;


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


        //equipped weapon
        if (weaponSlot != null)
        {
            Vector3 weaponPosition = new Vector3(154, 84, 0f); //fixed position for weapon slot
            var weaponObj = Instantiate(weaponSlot.GetItem().GetIcon(), Vector3.zero, Quaternion.identity, transform);
            weaponObj.GetComponent<RectTransform>().localPosition = weaponPosition;
            weaponObj.GetComponentInChildren<TextMeshProUGUI>().text = weaponSlot.GetItem().name;
            itemsDisplayed.Add(weaponSlot, weaponObj);

            //add unequip and drop buttons
            weaponObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                //find dropdown menu
                Transform panelTransform = weaponObj.transform.Find("DropdownMenu");
                //find first button + make visibile - unequip
                Transform firstButtonTransform = panelTransform.Find("FirstButton");
                firstButtonTransform.gameObject.SetActive(!firstButtonTransform.gameObject.activeSelf);
                Button firstButton = firstButtonTransform.GetComponent<Button>();
                AddButtonListener(firstButton, "Unequip", weaponSlot, 0); //index wont be used, using 0 as dummy
                //set drop button listener
                Transform secondButtonTransform = panelTransform.Find("SecondButton");
                secondButtonTransform.gameObject.SetActive(!secondButtonTransform.gameObject.activeSelf);
                Button secondButton = secondButtonTransform.GetComponent<Button>();
                AddButtonListener(secondButton, "DropEquippedW", weaponSlot, 0); //index wont be used
            });

        }

        //equipped armour
        if (armourSlot != null)
        {
            Vector3 armourPosition = new Vector3(154, -68, 0f); //fixed position for armour slot
            var armourObj = Instantiate(armourSlot.GetItem().GetIcon(), Vector3.zero, Quaternion.identity, transform);
            armourObj.GetComponent<RectTransform>().localPosition = armourPosition;
            armourObj.GetComponentInChildren<TextMeshProUGUI>().text = armourSlot.GetItem().name;
            itemsDisplayed.Add(armourSlot, armourObj);

            //add unequip and drop buttons
            armourObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                //find dropdown menu
                Transform panelTransform = armourObj.transform.Find("DropdownMenu");
                //find first button + make visibile - unequip
                Transform firstButtonTransform = panelTransform.Find("FirstButton");
                firstButtonTransform.gameObject.SetActive(!firstButtonTransform.gameObject.activeSelf);
                Button firstButton = firstButtonTransform.GetComponent<Button>();
                AddButtonListener(firstButton, "Unequip", armourSlot, 0); //index wont be used, using 0 as dummy
                //set drop button listener
                Transform secondButtonTransform = panelTransform.Find("SecondButton");
                secondButtonTransform.gameObject.SetActive(!secondButtonTransform.gameObject.activeSelf);
                Button secondButton = secondButtonTransform.GetComponent<Button>();
                AddButtonListener(secondButton, "DropEquippedA", armourSlot, 0); //index wont be used
            });
        }



        for (int i = 0; i < inventory.GetItems().Count; i++) //loading up the inventory on game start
        {
            int index = i; // capture the current value of i for the closure
            var item = inventory.GetItems(index); //cache the item for use in the listener
            var obj = Instantiate(inventory.GetItems(i).GetItem().GetIcon(), Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.GetItems(i).GetItem().name;

            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
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
                    AddButtonListener(firstButton, "Use", item, index);
                }
                else if(item.GetItem() is EquipmentObject)
                {
                    AddButtonListener(firstButton, "Equip", item, index);
                }

                //set drop button listener
                Transform secondButtonTransform = panelTransform.Find("SecondButton");
                secondButtonTransform.gameObject.SetActive(!secondButtonTransform.gameObject.activeSelf);

                Button secondButton = secondButtonTransform.GetComponent<Button>();

                if(item.GetItem() is HealthObject || item.GetItem() is EquipmentObject)
                {
                    AddButtonListener(secondButton, "Drop", item,  index);
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
            var item = inventory.GetItems(index); //cache the item for use in the listener
            var obj = Instantiate(inventory.GetItems(i).GetItem().GetIcon(), Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.GetItems(i).GetItem().name;

            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("Clicked on " + inventory.GetItems(index).GetItem().name);

                //find dropdown menu
                Transform panelTransform = obj.transform.Find("DropdownMenu");

                //get first button + make visibile - take item
                Transform buttonTransform = panelTransform.Find("FirstButton");
                buttonTransform.gameObject.SetActive(!buttonTransform.gameObject.activeSelf);

                Button takeButton = buttonTransform.GetComponent<Button>();
                AddButtonListener(takeButton, "Take", item, index);
            });

            itemsDisplayed.Add(inventory.GetItems(i), obj);
        }

    }

    private void AddButtonListener(Button button, string action, InventorySlot item, int index)
    {
        button.onClick.RemoveAllListeners(); //clear previous listeners to avoid stacking

        //set the text
        if(action != "DropEquippedW" && action != "DropEquippedA")
            button.GetComponentInChildren<TextMeshProUGUI>().text = action;
        else
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Drop";


        switch (action)
            {
                case "Use":
                    button.onClick.AddListener(() =>
                    {

                        bool okToDelete = item.GetItem().Use();

                        if (okToDelete)
                        {
                            inventory.GetItems().RemoveAt(index);
                            RefreshDisplay();
                        }


                        Debug.Log("Used " + item.GetItem().name);

                    });
                    break;
                case "Equip":
                    button.onClick.AddListener(() =>
                    {

                        bool okToEquip = item.GetItem().Use();


                        if (okToEquip)
                        {

                            //auto swap with currently equipped item
                            EquipmentObject equipment = (EquipmentObject)item.GetItem();
                            if (equipment.GetEquipmentType() == EquipmentType.Weapon)
                            {

                                if (weaponSlot != null) //swap
                                {
                                    inventory.Insert(weaponSlot.GetItem(), index);
                                    weaponSlot = item;
                                }
                                else //first time equipping
                                {
                                    weaponSlot = item;
                                    inventory.GetItems().RemoveAt(index);
                                }

                                //find text and modify to show equipped weapon
                                GameObject.Find("WeaponText").GetComponent<TextMeshProUGUI>().text = $"+{equipment.GetBonusValue()} damage";
                            }
                            else if (equipment.GetEquipmentType() == EquipmentType.Armour)
                            {
                                if (armourSlot != null) //swap
                                {
                                    inventory.Insert(armourSlot.GetItem(), index);
                                    armourSlot = item;
                                }
                                else //first time equipping
                                {
                                    armourSlot = item;
                                    inventory.GetItems().RemoveAt(index);
                                }

                                //find text and modify to show equipped armour
                                GameObject.Find("ArmourText").GetComponent<TextMeshProUGUI>().text = $"+{equipment.GetBonusValue()} armour";

                            }
                            Debug.Log("Equipped " + item.GetItem().name);

                            RefreshDisplay();
                        }



                    });
                    break;
                case "Unequip":
                    button.onClick.AddListener(() =>
                    {

                        bool okToUnequip = inventory.AddItem(item.GetItem());
                        if (okToUnequip)
                        {
                            if (item.GetItem() is EquipmentObject) //should be always true here
                            {
                                EquipmentObject equipment = (EquipmentObject)item.GetItem();
                                if (equipment.GetEquipmentType() == EquipmentType.Weapon)
                                {
                                    weaponSlot = null;
                                    //find text and modify to show no equipped weapon
                                    GameObject.Find("WeaponText").GetComponent<TextMeshProUGUI>().text = "no weapon equipped";
                                }
                                else if (equipment.GetEquipmentType() == EquipmentType.Armour)
                                {
                                    armourSlot = null;
                                    //find text and modify to show no equipped armour
                                    GameObject.Find("ArmourText").GetComponent<TextMeshProUGUI>().text = "no armour equipped";
                                }
                            }
                            RefreshDisplay();
                        }

                        Debug.Log("Unequipped " + item.GetItem().name);
                    });
                    break;
                case "Drop":
                    button.onClick.AddListener(() =>
                    {
                        Debug.Log("Dropped " + item.GetItem().name);
                        inventory.GetItems().RemoveAt(index);
                        RefreshDisplay();
                    });
                    break;
                case "DropEquippedW":
                    button.onClick.AddListener(() =>
                    {
                        Debug.Log("Dropped equipped " + item.GetItem().name);
                        weaponSlot = null;
                        //find text and modify to show no equipped weapon
                        GameObject.Find("WeaponText").GetComponent<TextMeshProUGUI>().text = "no weapon equipped";
                        RefreshDisplay();
                    });
                    break;
                case "DropEquippedA":
                    button.onClick.AddListener(() =>
                    {
                        Debug.Log("Dropped equipped " + item.GetItem().name);
                        armourSlot = null;
                        //find text and modify to show no equipped armour
                        GameObject.Find("ArmourText").GetComponent<TextMeshProUGUI>().text = "no armour equipped";
                        RefreshDisplay();
                    });
                    break;
            case "Take":
                    button.onClick.AddListener(() =>
                    {
                        if (playerInventory.AddItem(item.GetItem()))
                        {
                            inventory.GetItems().RemoveAt(index);
                            RefreshDisplay();
                            Debug.Log("Took " + item.GetItem().name);
                        }
                        else
                        {
                            Debug.Log("Not enough space in player inventory to take " + item.GetItem().name);
                        }

                    });
                    break;
                default:
                    Debug.LogWarning("Action " + action + " not recognized.");
                    break;
            }


    }

}
