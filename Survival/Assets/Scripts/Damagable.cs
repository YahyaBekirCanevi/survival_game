using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    [SerializeField] private float maxHP;
    [SerializeField] private float currentHP;
    [SerializeField] private int dropCount = 3;
    public float timeToDestroy { get; set; }
    public bool canDestroyed = false, damaged = false, destroyParent = false;
    [SerializeField] private GameObject drop;
    public float HP
    {
        get => currentHP;
        private set => currentHP = value;
    }
    private void Start()
    {
        currentHP = maxHP;
    }
    private void Update()
    {
        if (currentHP > maxHP) currentHP = maxHP;
        canDestroyed = currentHP < 0;
        if (canDestroyed)
        {
            canDestroyed = false;
            Destroy(timeToDestroy);
        }
    }
    public void TakeDamage(float damage)
    {
        damaged = true;
        HP -= damage;
        StartCoroutine(CalmDown());
    }
    IEnumerator CalmDown()
    {
        yield return new WaitForSeconds(1);
        damaged = false;
    }
    public void Destroy(float time) => StartCoroutine(OnDestroyed(time));
    IEnumerator OnDestroyed(float time)
    {
        yield return new WaitForSeconds(time);
        if (drop != null)
        {
            for (int i = 0; i < dropCount; i++)
                Instantiate(drop, drop.transform.position + transform.position + Vector3.up + Random.insideUnitSphere, drop.transform.rotation);
        }
        Destroy(destroyParent ? transform.parent.gameObject : this.gameObject);
    }
}