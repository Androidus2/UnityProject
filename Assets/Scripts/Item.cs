using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public ItemObject item;

    [SerializeField]
    private string _prompt;
    public string InteractionPoint => _prompt;

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Adding item: to inventory");
        Destroy(gameObject); //destroy the item
        return true;
    }
}
