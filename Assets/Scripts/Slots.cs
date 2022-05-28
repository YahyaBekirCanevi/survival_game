using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slots : MonoBehaviour
{
    [SerializeField] Color selectedColor;
    [SerializeField] private int selectedIndex = 0;
    [SerializeField] private Image[] slots = new Image[4];
    void Update()
    {
        ChooseSlot(KeyCode.Alpha1, 1);
        ChooseSlot(KeyCode.Alpha2, 2);
        ChooseSlot(KeyCode.Alpha3, 3);
        ChooseSlot(KeyCode.Alpha4, 4);
    }
    void ChooseSlot(KeyCode key, int index)
    {
        if (selectedIndex == index || index > 4) return;
        index--;
        if (Input.GetKeyDown(key))
        {
            slots[index].color = selectedColor;
            selectedIndex = index + 1;
            if (transform.childCount > index)
                transform.GetChild(index).gameObject.SetActive(true);
        }
        else
        {
            slots[index].color = Color.white;
            if (transform.childCount > index)
                transform.GetChild(index).gameObject.SetActive(false);
        }
    }
}