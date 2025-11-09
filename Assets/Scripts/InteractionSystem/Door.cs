using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string _prompt;
    public string InteractionPoint => _prompt;

    public bool Interact(Interactor interactor)
    {
        //work in progress
        Debug.Log("Opening door");
        return true;
    }
}
