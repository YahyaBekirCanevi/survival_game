using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    public GameObject arrow;
    private Arrow _arrow;
    private bool released = true;
    [SerializeField] private Transform arrowParent;
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.V))
        {
            Release();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && released)
            CreateArrow();
        if (Input.GetKeyUp(KeyCode.Mouse0) && !released)
            Fire();

        if (cc.zoom)
            cc.Zoom();
        else
            cc.UnZoom();
        anim.SetBool("attack", Input.GetKey(KeyCode.Mouse0) && !released);
        anim.SetBool("release", released);
    }
    private void CreateArrow()
    {
        GameObject go = Instantiate(arrow);
        _arrow = go.GetComponent<Arrow>();
        _arrow.parent = arrowParent;
        released = false;
    }
    private void Fire() => _arrow.fire = released = true;
    private void Release()
    {
        released = true;
        _arrow.fire = false;
        anim.SetBool("attack", false);
        anim.SetBool("release", true);
        FinishAttack();
        Destroy(_arrow.gameObject);
        TrueAttack = false;
    }
    public override void Damage(float rate)
    {
        if (isAttacking && castfromCamera)
        {
            base.Damage(rate);
            RaycastHit hit = cc.CastHit(range, damagable);
            GameObject a = Instantiate(arrow, hit.point - (hit.normal * .25f), Quaternion.Euler(-hit.normal));
            Arrow ar = a.GetComponent<Arrow>();
            ar.parent = this.transform;
            ar.punctured = true;
        }
    }
}