using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, int> objectTags;
    [SerializeField] private LayerMask collectable;
    [SerializeField] private Text collectableText;
    private CameraController cam;
    private void Start()
    {
        objectTags = new Dictionary<string, int>();
        cam = GameObject.FindObjectOfType<CameraController>();
    }
    private void Update()
    {
        Collect(cam.Cast(3, collectable));
    }
    private void Collect(Collider collect)
    {
        if (collect)
        {
            collectableText.enabled = true;
            if (Input.GetKeyDown(KeyCode.F))
            {
                this.AddObject(collect.gameObject.tag);
                Destroy(collect.gameObject);
            }
        }
        else
            collectableText.enabled = false;
    }
    private void AddObject(string tag)
    {
        if (objectTags.ContainsKey(tag))
            objectTags[tag]++;
        else
            objectTags.Add(tag, 1);
    }
    public int GetCountOfObject(string tag) => objectTags.ContainsKey(tag) ? objectTags[tag] : 0;
}