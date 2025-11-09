using Unity.VisualScripting;
using UnityEngine;

public interface IInteractable
{
    
    public string InteractionPoint { get; }

    public bool Interact (Interactor interactor);

}
