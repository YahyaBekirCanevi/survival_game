using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    protected Animator anim;
    protected PlayerController pc;
    protected CameraController cc;
    protected bool isAttacking = false;
    public bool TrueAttack { get; protected set; }
    protected int noHitAttack = 0;
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected WeaponType weaponType;
    [SerializeField] protected Image crosshair;
    [SerializeField] protected LayerMask damagable;
    public Collider castfromCamera { get; protected set; }
    protected void Start()
    {
        anim = GetComponent<Animator>();
        pc = transform.GetComponentInParent<PlayerController>();
        cc = GameObject.FindObjectOfType<CameraController>();
        InvokeRepeating("Cast", .1f, .2f);
    }
    protected virtual void Update()
    {
        if (castfromCamera != null)
            TrueAttack = castfromCamera.tag == AttackTag();
        else
            TrueAttack = false;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isAttacking = true;
            if (castfromCamera != null)
            {
                anim.SetInteger("attackType", TrueAttack ? 0 : 1);
                noHitAttack = 0;
            }
            else
            {
                anim.SetInteger("attackType", (noHitAttack++) % 2);
            }
        }
        crosshair.color = TrueAttack ? Color.red : Color.white;
        Vector3 speed = pc.GetComponent<Rigidbody>().velocity;
        speed.y = 0;
        anim.SetFloat("speed", pc.IsGrounded ? speed.normalized.magnitude : 0);
        if (weaponType != WeaponType.Bow) anim.SetBool("attack", isAttacking);
    }
    public virtual void Damage(float rate)
    {
        if (isAttacking && castfromCamera)
        {
            castfromCamera.gameObject.GetComponent<Damagable>().timeToDestroy = 1 - rate;
            castfromCamera.gameObject.GetComponent<Damagable>().TakeDamage(damage);
            FinishAttack();
        }
    }
    private void Cast()
    {
        castfromCamera = cc.Cast(range, damagable);
    }
    protected string AttackTag()
    {
        string tag = "";
        switch (weaponType)
        {
            case WeaponType.Axe:
                {
                    tag = "Tree";
                    break;
                }
            case WeaponType.Pickaxe:
                {
                    tag = "Stone";
                    break;
                }
            case WeaponType.Bow:
                {
                    tag = "Animal";
                    break;
                }
            default: break;
        }
        return tag;
    }
    public void FinishAttack() => isAttacking = false;
    protected enum WeaponType { None, Axe, Pickaxe, Bow }
}