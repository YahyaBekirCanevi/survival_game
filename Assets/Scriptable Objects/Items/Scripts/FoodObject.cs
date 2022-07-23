using UnityEngine;

[CreateAssetMenu(fileName = "New Food Object", menuName = "Inventory System/Items/Food")]
public class FoodObject : ItemObject
{
    public float restoreHealthValue;
    public void Awake()
    {
        itemType = ItemType.Food;
    }
}
