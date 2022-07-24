using UnityEngine;
using UnityEngine.UI;

public class DetailDisplay : MonoBehaviour
{
    [SerializeField] private bool isOpen = false;
    public bool Open { get => isOpen; set => isOpen = value; }
    private ItemObject selected;
    [SerializeField] private Image icon;
    [SerializeField] private TMPro.TMP_Text title, description;
    private void Update()
    {
        HideChildren();
    }
    public void Set(ItemObject item)
    {
        if (Open)
        {
            icon.sprite = item.image;
            title.text = item.name;
            description.text = item.description;
            selected = item;
        }
    }
    public void Use()
    {
        print($"{selected.name} used!");
        CloseMenu();
    }
    public void CloseMenu()
    {
        Open = false;
    }

    private void HideChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(Open);
        }
    }
}
