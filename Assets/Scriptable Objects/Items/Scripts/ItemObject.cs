using UnityEngine;

public enum ItemType { Food, Collectable, Weapon, Default }
public abstract class ItemObject : ScriptableObject
{
    public new string name;
    public GameObject prefab;
    public Sprite image;
    public ItemType itemType;
    [TextArea(5, 10)] public string description;
}
