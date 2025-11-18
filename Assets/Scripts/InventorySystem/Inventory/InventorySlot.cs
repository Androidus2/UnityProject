[System.Serializable]
public class InventorySlot
{
    private ItemObject item;
    //here, im choosing to not have 'slots'; repeats of the same objects (i dont expect there to be many) will take each 1 slot
    //if we decide to add this later on, it starts with public int amount;
    public InventorySlot(ItemObject item)
    {
        this.item = item;
    }

    public ItemObject GetItem()
    {
        return item;
    }
}