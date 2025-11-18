using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ItemObject item;

    public void Interact(Interactor interactor, InventoryObject inventory)
    {
        Debug.Log("Adding item: " + item.name + " to inventory");
        inventory.AddItem(this.GetItem());
        Destroy(gameObject); //destroy the item
    }
    public ItemObject GetItem()
    {
        return item;
    }
}
