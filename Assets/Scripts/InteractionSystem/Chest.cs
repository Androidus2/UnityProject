using UnityEngine;

public class Chest : InteractableBase
{
       public override void Interact(Interactor interactor, InventoryObject inventory)
    {
        //lockpicking mechanic
        //open the chest ui
        Debug.Log("Opening chest");
    }
}
