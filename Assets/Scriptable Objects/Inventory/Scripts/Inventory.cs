using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    public ItemObject Selected;
    public List<ItemObject> Container = new List<ItemObject>();
    public void AddItem(ItemObject item)
    {
        int index = -1;
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i] == item)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            Container.Add(item);
        }
        else
        {
            if (item is CollectableObject)
            {
                (Container[index] as CollectableObject).AddAmount(1);
            }
            else { }
        }

    }
}