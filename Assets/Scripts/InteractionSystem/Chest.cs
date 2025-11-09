using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string _prompt;
    public string InteractionPoint => _prompt;

    public bool Interact(Interactor interactor)
    {
        //lockpicking mechanic
        //open the chest ui
        Debug.Log("Opening chest");
        return true;
    }
}
