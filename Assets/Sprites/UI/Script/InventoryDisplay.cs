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
    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite defaultSprite;
    private void Awake()
    {
        context = GetComponent<RectTransform>();
        for (int i = 0; i < 8; i++)
        {
            if (inventory.Container.Count - 1 < i)
            {
                images[i].transform.GetChild(0).GetComponent<Image>().sprite = defaultSprite;
            }
            else
            {
                images[i].transform.GetChild(0).GetComponent<Image>().sprite = inventory.Container[i].image;
            }
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

    private void Activate()
    {
        isOpen = !isOpen;

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
