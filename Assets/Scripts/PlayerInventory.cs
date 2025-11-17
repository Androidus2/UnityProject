using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : Interactor
{
    InputAction interactButton;
    public InventoryObject inventory;

    private void Start()
    {
        interactButton = InputSystem.actions.FindAction("Interact");

    }

    //this is different from the interactor by being on its own layer
    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        if (_numFound > 0)
        {
            var interactable = _colliders[0].GetComponent<IInteractable>(); //find the mono behaviour which implements the interface
            
            //if the interactable is an item, we add it to inventory and destroy it as well
            if (interactable != null && interactButton.WasPressedThisFrame()) {
                
                try{
                    Item item = (Item)interactable;
                    inventory.AddItem(item.GetItem());  //items should be on item layer, but in order to make sure, we're adding a try
                    Destroy(_colliders[0].gameObject);
                }
                catch {  }
            }
        }

       
       
    }

    //for testing purposes, we clear the inventory
    private void OnApplicationQuit()
    {
        inventory.items.Clear();    
    }

}
