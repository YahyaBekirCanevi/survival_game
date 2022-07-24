using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private LayerMask collectable;
    [SerializeField] private Text collectableText;
    [SerializeField] private CameraController cam;
    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        enabled = newGameState == GameState.Gameplay;
    }
    private void Update()
    {
        Collect(cam.Cast(3, collectable));
    }
    private void Collect(Collider collect)
    {
        if (collect == null)
        {
            collectableText.enabled = false;
            return;
        }
        collectableText.enabled = true;
        var item = collect.GetComponent<Item>();
        if (Input.GetKeyDown(KeyCode.F))
        {
            inventory.AddItem(item.item);
            Destroy(collect.gameObject);
        }
    }
    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
        inventory.Selected = null;
    }
}