using UnityEngine;
using UnityEngine.Rendering;

using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField]
    private Transform _interactionPoint;

    [SerializeField]
    private float _interactionPointRadius = 0.5f; //interact radius / area

    [SerializeField]
    private LayerMask _interactableMask;

    private readonly Collider[] _colliders = new Collider[3];

    [SerializeField]
    private int _numFound; //number of colliders found - serialized for viewing

    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        if(_numFound > 0)
        {
            var interactable = _colliders[0].GetComponent<IInteractable>(); //find the mono behaviour which implements the interface

            if (interactable != null && Keyboard.current.eKey.wasPressedThisFrame) { //to add controller key?
                interactable.Interact(this);
            
            }

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
