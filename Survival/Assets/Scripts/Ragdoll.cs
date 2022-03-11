using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Collider[] MainColliders;
    private Collider[] DollColliders;
    private Rigidbody[] DollBodies;
    [SerializeField] private MonoBehaviour controlScript;
    private void Awake()
    {
        MainColliders = GetComponents<Collider>();
        DollColliders = GetComponentsInChildren<Collider>();
        DollBodies = GetComponentsInChildren<Rigidbody>();
    }
    private void Start()
    {
        OnRagdoll(false);
    }
    public void OnRagdoll(bool active)
    {
        foreach (var collider in DollColliders)
            collider.enabled = active;
        foreach (var rb in DollBodies)
        {
            rb.detectCollisions = active;
            rb.isKinematic = !active;
        }
        foreach (var collider in MainColliders)
            collider.enabled = !active;
        controlScript.enabled = !active;
        GetComponent<Animator>().enabled = !active;
    }
}