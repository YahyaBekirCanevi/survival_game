using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    private bool isOpen = true;
    private RectTransform context;
    private Vector2 destinedSize;
    private List<Transform> images;
    [SerializeField] private float width, height;
    [SerializeField] private Inventory inventory;
    [SerializeField] private DetailDisplay detailMenu;
    [SerializeField] private Color backgroundColor, headerColor, selectedSlotBackgroundColor, slotBackgroundColor;
    [SerializeField] private Sprite defaultSprite;
    private void Awake()
    {
        context = GetComponent<RectTransform>();
        images = new List<Transform>();
        var slotsObj = transform.Find("Slots");
        for (int i = 0; i < slotsObj.childCount; i++)
        {
            images.Add(slotsObj.GetChild(i));
        }
        Activate();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Activate();
        }
    }

    private void SetSelected(ItemObject item)
    {
        detailMenu.Open = true;
        detailMenu.Set(item);
        if (inventory.Selected == item) return;
        inventory.Selected = item;
        DrawSlots();
    }
    private void DrawSlots()
    {
        Color color = Color.white;

        transform.GetChild(0).GetComponent<Image>().color = backgroundColor;
        transform.GetChild(1).GetComponent<Image>().color = headerColor;
        for (int i = 0; i < 8; i++)
        {
            if (inventory.Container.Count - 1 < i)
            {
                color.a = 0;
                images[i].GetComponent<Image>().color = slotBackgroundColor;
                images[i].GetChild(0).GetComponent<Image>().sprite = defaultSprite;
                images[i].GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            }
            else
            {
                color.a = 1;
                images[i].GetComponent<Image>().color =
                    i == inventory.Container.IndexOf(inventory.Selected)
                        ? selectedSlotBackgroundColor
                        : slotBackgroundColor;
                images[i].GetChild(0).GetComponent<Image>().sprite = inventory.Container[i].image;
                int x = i;
                images[i].GetChild(1).GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        SetSelected(inventory.Container[x]);
                    }
                );
            }
            images[i].GetChild(0).GetComponent<Image>().color = color;
        }
    }
    private void Activate()
    {
        isOpen = !isOpen;
        DrawSlots();

        GameState newGameState = isOpen ? GameState.Paused : GameState.Gameplay;
        GameStateManager.Instance.SetState(newGameState);

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
        detailMenu.Open = isOpen ? detailMenu.Open : false;

        destinedSize = isOpen ? new Vector2(width, height) : new Vector2(0, 0);
        if (isOpen) HideChildren();
        context.sizeDelta = destinedSize;
        if (!isOpen) HideChildren();
    }
    private void HideChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject == detailMenu.gameObject)
                continue;
            transform.GetChild(i).gameObject.SetActive(isOpen);
        }
    }
}
