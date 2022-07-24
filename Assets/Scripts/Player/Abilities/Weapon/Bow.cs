using UnityEngine;

public class Bow : Weapon
{
    public GameObject arrow;
    private Arrow _arrow;
    private bool released = true;
    [SerializeField] private Transform arrowParent;
    protected override void Awake()
    {
        base.Awake();
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

        if (Input.GetKey(KeyCode.Mouse1))
            cam.Zoom();
        else
            cam.UnZoom();
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
            RaycastHit hit = cam.CastHit(range, damagable);
            GameObject a = Instantiate(arrow, hit.point - (hit.normal * .25f), Quaternion.Euler(-hit.normal));
            Arrow ar = a.GetComponent<Arrow>();
            ar.parent = this.transform;
            ar.punctured = true;
        }
    }
}