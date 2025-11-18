using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    private InputAction interactButton;

    [SerializeField]
    protected Transform interactionPoint;

    [SerializeField]
    protected float interactionPointRadius = 0.5f; //interact radius / area

    [SerializeField]
    protected LayerMask interactableMask;

    protected Collider[] colliders = new Collider[3];

    [SerializeField]
    protected int numFound; //number of colliders found - serialized for viewing


    //inventory system

    [SerializeField]
    private InventoryObject inventory;

    
    private void Start()
    {
        interactButton = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {
        //interactables
        numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionPointRadius, colliders, interactableMask);

        if(numFound > 0)
        {
           
            var interactable = colliders[0].GetComponent<IInteractable>(); //find the mono behaviour which implements the interface

            if (interactable != null && interactButton.WasPressedThisFrame()) { //to add controller key?
                interactable.Interact(this, inventory);

            
            }

        }

    }

    private void OnDrawGizmos() //to view the interaction sphere
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionPointRadius);
    }

    //for testing purposes, we clear the inventory
    private void OnApplicationQuit()
    {
        inventory.GetItems().Clear();
    }


}
