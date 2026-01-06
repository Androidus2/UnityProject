using DG.Tweening;
using UnityEngine;

public class EndStatue : InteractableBase
{
    [SerializeField]
    private InventoryObject playerInventory;

    [SerializeField] 
    private GameObject angelWings;



    protected override void Awake()
    {
        base.Awake();
        if (playerInventory == null)
        {
            playerInventory = Resources.Load<InventoryObject>("Inventory/PlayerInventory");
        }

        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory ScriptableObject NOT FOUND");
        }
    }

    public override void Interact(Interactor interactor, InventoryObject inventory)
    {
        //if angel wings in inventory
        //end game lol

        Debug.Log("At the end statue");

        if (HasAngelWings())
        {
            //find them and display them
            if (angelWings != null) {
                angelWings.SetActive(true);
            }
            //cinematic view?
            //transition to end scene
            Debug.Log("You have the Angel Wings! Game Over!");

        }
        else
        {
            Debug.Log("You need the Angel Wings to activate the statue.");
        }
    }


    private bool HasAngelWings()
    {
        foreach (var item in playerInventory.GetItems())
        {
            if (item.GetItem().name == "Angel Wings")
            {
                return true;
            }
        }
        return false;
    }

}
