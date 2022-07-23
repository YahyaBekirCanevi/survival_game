using UnityEngine;

[CreateAssetMenu(fileName = "New Collectable Object", menuName = "Inventory System/Items/Collectable")]
public class CollectableObject : ItemObject
{
    public int amount;
    public void Awake()
    {
        itemType = ItemType.Collectable;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
    public void RemoveAmount(int value)
    {
        amount -= value;
        amount = amount < 0 ? 0 : amount;
    }
}
