using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public bool fire = false;
    public bool punctured = false;
    private bool fired = false;
    public Transform parent { get; set; }
    void Update()
    {
        if (fire)
        {
            fire = false;
            fired = true;
        }
        if (!fired && parent)
        {
            if (!punctured)
            {
                transform.position = parent.position;
                transform.forward = -parent.up;
            }
            else
                Destroy(gameObject, 3);
        }
        else Destroy(gameObject);
    }
}