using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
       public void Interact(Interactor interactor, InventoryObject inventory)
    {
        //lockpicking mechanic
        //open the chest ui
        Debug.Log("Opening chest");
    }
}
