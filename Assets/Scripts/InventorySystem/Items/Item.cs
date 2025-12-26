using UnityEngine;

public class Item : InteractableBase
{
    [SerializeField]
    private ItemObject item;

    public override void Interact(Interactor interactor, InventoryObject inventory)
    {
        Debug.Log("Adding item: " + item.name + " to inventory");
        if(inventory.AddItem(GetItem()))
            Destroy(gameObject); //destroy the item
    }
    public ItemObject GetItem()
    {
        return item;
    }
}
