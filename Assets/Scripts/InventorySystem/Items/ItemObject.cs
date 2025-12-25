using UnityEngine;

public enum ItemType
{
    //what type of items we want in our game
    Health,
    Mission,
    Equipment,
    Money
}
public abstract class ItemObject : ScriptableObject
{
    [SerializeField]
    protected GameObject icon; 

    [SerializeField]
    protected ItemType type;

    [SerializeField]
    protected int price; //for vendors

    public GameObject GetIcon()
    {
        return icon;
    }

    public int GetPrice()
    {
        //karma markup implementation could go here!!
        return price;
    }

    public abstract bool Use();
}
