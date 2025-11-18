using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ItemObject item;

    public void Interact(Interactor interactor)
    {
        Debug.Log("Adding item: " + item.name + " to inventory");
        Destroy(gameObject); //destroy the item
    }
    public ItemObject GetItem()
    {
        return item;
    }
}
