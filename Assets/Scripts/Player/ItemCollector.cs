using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private LayerMask collectable;
    [SerializeField] private Text collectableText;
    [SerializeField] private CameraController cam;
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
        print($"{collect.gameObject.name} {item.item.name}");
        if (Input.GetKeyDown(KeyCode.F))
        {
            inventory.AddItem(item.item);
            Destroy(collect.gameObject);
        }
    }
    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }
}