using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InventoryObject))]
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

    [SerializeField]
    private GameObject interactTextObject; // The E to interact button

    private Tween interactTextTween;

    //inventory system
    private InventoryObject inventory;

    private InteractableBase currentlyOutlined;

    private void Awake()
    {
        inventory = GetComponent<InventoryObject>();
    }

    private void Start()
    {
        interactButton = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {
        int previousNumFound = numFound;
        //interactables
        numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionPointRadius, colliders, interactableMask);

        if(previousNumFound == 0 && numFound > 0)
        {
            interactTextObject.SetActive(true);
            interactTextTween?.Kill();
            interactTextTween = interactTextObject.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
        }
        else if(previousNumFound > 0 && numFound == 0)
        {
            interactTextTween?.Kill();
            interactTextTween = interactTextObject.transform.DOScale(0, 0.2f).SetEase(Ease.InBack).OnComplete(() => interactTextObject.SetActive(false));
        }

            InteractableBase interactable = null;
        if (numFound > 0)
            interactable = colliders[0].GetComponent<InteractableBase>();

        // Only disable the previous outline if it's different from the new one
        if (currentlyOutlined != null && currentlyOutlined != interactable)
        {
            currentlyOutlined.ShowOutline(false);
            currentlyOutlined = null;
        }

        // Enable the new outline if there is one
        if (interactable != null)
        {
            interactable.ShowOutline(true);
            currentlyOutlined = interactable;

            // Check if we have interacted
            if (interactButton.WasPressedThisFrame())
                interactable.Interact(this, inventory);
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
        inventory.ClearInventory();
    }


}
