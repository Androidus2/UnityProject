using UnityEngine;
using UnityEngine.Rendering;

using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class Interactor : MonoBehaviour
{
    [SerializeField]
    protected Transform _interactionPoint;

    [SerializeField]
    protected float _interactionPointRadius = 0.5f; //interact radius / area

    [SerializeField]
    protected LayerMask _interactableMask;

    protected Collider[] _colliders = new Collider[3];

    [SerializeField]
    protected int _numFound; //number of colliders found - serialized for viewing

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

    private void OnDrawGizmos() //to view the interaction sphere
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
