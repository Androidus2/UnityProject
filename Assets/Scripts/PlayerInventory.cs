using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : Interactor
{

    public InventoryObject inventory;

    //this is different from the interactor by being on its own layer
    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        if (_numFound > 0)
        {
            var interactable = _colliders[0].GetComponent<IInteractable>(); //find the mono behaviour which implements the interface
            
            //if the interactable is an item, we add it to inventory and destroy it as well
            if (interactable != null && Keyboard.current.eKey.wasPressedThisFrame)
            {   Debug.Log("e was pressed");
                interactable.Interact(this);
            }
        }
       
    }
}
