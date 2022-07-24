using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    private bool isOpen = true;
    private RectTransform context;
    private Vector2 destinedSize;
    [SerializeField] private float width, height;
    [SerializeField] private Inventory inventory;
    [SerializeField] private List<GameObject> images;
    [SerializeField] private Color backgroundColor, headerColor, selectedSlotBackgroundColor, slotBackgroundColor;
    [SerializeField] private Sprite defaultSprite;
    private void Awake()
    {
        context = GetComponent<RectTransform>();
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
                images[i].transform.GetComponent<Image>().color = slotBackgroundColor;
                images[i].transform.GetChild(0).GetComponent<Image>().sprite = defaultSprite;
                images[i].transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            }
            else
            {
                color.a = 1;
                images[i].transform.GetComponent<Image>().color =
                    i == inventory.Container.IndexOf(inventory.Selected)
                        ? selectedSlotBackgroundColor
                        : slotBackgroundColor;
                images[i].transform.GetChild(0).GetComponent<Image>().sprite = inventory.Container[i].image;
                int x = i;
                images[i].transform.GetChild(1).GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        SetSelected(inventory.Container[x]);
                    }
                );
            }
            images[i].transform.GetChild(0).GetComponent<Image>().color = color;
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

        destinedSize = isOpen ? new Vector2(width, height) : new Vector2(0, 0);
        if (isOpen) HideChildren();
        context.sizeDelta = destinedSize;
        if (!isOpen) HideChildren();
    }
    private void HideChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(isOpen);
        }
    }
}
