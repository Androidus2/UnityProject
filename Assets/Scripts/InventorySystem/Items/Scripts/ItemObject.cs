using UnityEngine;

public enum ItemType
{
    //what type of items we want in our game
    Health,
    Mission,
    Equipment
}
public abstract class ItemObject : ScriptableObject
{
    [SerializeField]
    protected GameObject icon;

    [SerializeField]
    protected ItemType type;

    [TextArea(15,20)]
    [SerializeField]
    protected string description;

    public GameObject GetIcon()
    {
        return icon;
    }
}
