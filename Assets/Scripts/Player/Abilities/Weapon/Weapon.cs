using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    protected Animator anim;
    protected PlayerController pc;
    protected CameraController cam;
    protected bool isAttacking = false;
    public bool TrueAttack { get; protected set; }
    protected int noHitAttack = 0;
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected WeaponType weaponType;
    [SerializeField] protected Image crosshair;
    [SerializeField] protected LayerMask damagable;
    public Collider castfromCamera { get; protected set; }
    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        pc = transform.GetComponentInParent<PlayerController>();
        cam = GameObject.FindObjectOfType<CameraController>();
        InvokeRepeating("Cast", .1f, .2f);
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        anim.SetFloat("speed", 0);
        enabled = newGameState == GameState.Gameplay;
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
        anim.SetFloat("speed", pc.MovementSpeed);
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
        castfromCamera = cam.Cast(range, damagable);
    }
    protected string AttackTag()
    {
        switch (weaponType)
        {
            case WeaponType.Axe:
                return "Tree";
            case WeaponType.Pickaxe:
                return "Stone";
            case WeaponType.Bow:
                return "Animal";
            default:
                return "";
        }
    }
    public void FinishAttack() => isAttacking = false;
    protected enum WeaponType { None, Axe, Pickaxe, Bow }
}