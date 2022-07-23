using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Object", menuName = "Inventory System/Items/Weapon")]
public class WeaponObject : ItemObject
{
    public float damage;
    public float range;
    public void Awake()
    {
        itemType = ItemType.Weapon;
    }
}
