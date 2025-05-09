using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private float speed = 30f;
    [SerializeField] private float gravity = 1f;
    [SerializeField] private float maxLifetime = 10f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private GameObject hitEffect;

    private bool isFired = false;
    private Vector3 velocity;
    private Transform parent;
    private float lifetime;

    public void Initialize(Transform bowTransform, float initialSpeed)
    {
        parent = bowTransform;
        speed = initialSpeed;
        transform.position = parent.position;
        transform.rotation = parent.rotation;
    }

    public void Fire()
    {
        isFired = true;
        velocity = transform.forward * speed;
        lifetime = 0f;
    }

    private void Update()
    {
        if (!isFired)
        {
            if (parent != null)
            {
                transform.position = parent.position;
                transform.rotation = parent.rotation;
            }
            return;
        }

        // Update lifetime
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Apply gravity
        velocity += Vector3.down * gravity * Time.deltaTime;

        // Move arrow
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity.normalized);

        // Check for hits
        CheckForHits();
    }

    private void CheckForHits()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, velocity.normalized, out hit, velocity.magnitude * Time.deltaTime, hitLayers))
        {
            // Handle hit
            HandleHit(hit);
        }
    }

    private void HandleHit(RaycastHit hit)
    {
        // Spawn hit effect
        if (hitEffect != null)
        {
            Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

        // Check for damagable object
        Damagable damagable = hit.collider.GetComponent<Damagable>();
        if (damagable != null)
        {
            damagable.TakeDamage(damage);
        }

        // Stick arrow to surface
        transform.position = hit.point;
        transform.rotation = Quaternion.LookRotation(hit.normal);
        velocity = Vector3.zero;
        isFired = false;

        // Destroy arrow after delay
        Destroy(gameObject, 5f);
    }

    private void OnDrawGizmos()
    {
        if (isFired)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, velocity.normalized * 2f);
        }
    }
}