using UnityEngine;

enum MovementType { Idle, Walk, Run, Crouch, Jump }

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    #region variables
    private float yScale;
    [Header("Movement")]
    [ShowOnly, SerializeField] private float speed;
    [ShowOnly, SerializeField] private Vector3 move;
    [ShowOnly, SerializeField] private MovementType movementType;
    [ShowOnly, SerializeField] private bool isGrounded, isCrouching, isRunning, isJumped;
    [SerializeField] private float walkSpeed = 8;
    [SerializeField] private float lowSpeed = 4;
    [SerializeField] private float runSpeed = 12;
    [SerializeField] private float jumpStrength = 8;
    [SerializeField] private Transform body;

    [Header("Drag")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;
    private CapsuleCollider cc;
    private Rigidbody rb;
    #endregion
    private float GroundDistance
    {
        get => Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 30) ? hit.distance : 30;
    }
    public bool IsGrounded { get => isGrounded; }
    public float MovementSpeed { get => move.normalized.magnitude; }
    private bool Grounded
    {
        get => GroundDistance <= 1.1f;
    }
    void Awake()
    {
        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        yScale = cc.height;
        movementType = MovementType.Idle;
        rb.freezeRotation = true;
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
    void Update()
    {
        Inputs();
        ControlDrag();
    }

    private void Inputs()
    {
        float ver = Input.GetAxisRaw("Vertical");
        float hor = Input.GetAxisRaw("Horizontal");
        move = transform.forward * ver + transform.right * hor;

        isCrouching = Input.GetKey(KeyCode.LeftControl);
        isJumped = Input.GetKeyDown(KeyCode.Space);
        isRunning = Input.GetKey(KeyCode.LeftShift);
    }

    private void ControlDrag()
    {
        rb.drag = isGrounded ? (move.magnitude == 0 ? 200 : groundDrag) : airDrag;
    }

    void FixedUpdate()
    {
        isGrounded = Grounded;

        speed = CalculateSpeed();

        Move();
        Jump();

        cc.height = isCrouching ? yScale * 0.5f : yScale;
        body.localScale = .5f * cc.height * Vector3.one;
    }
    private MovementType GetMovementType()
    {
        if (!isGrounded)
            return MovementType.Jump;
        if (isCrouching)
            return MovementType.Crouch;
        if (isRunning)
            return MovementType.Run;
        if (move.magnitude > 0)
            return MovementType.Walk;
        return MovementType.Idle;
    }
    private float CalculateSpeed()
    {
        movementType = GetMovementType();
        switch (movementType)
        {
            case MovementType.Idle:
                return 0;
            case MovementType.Walk:
                return walkSpeed;
            case MovementType.Run:
                return runSpeed;
            case MovementType.Crouch:
                return lowSpeed;
            case MovementType.Jump:
                return walkSpeed;
            default:
                return 0;
        }
    }
    private void Move()
    {
        move = (isGrounded ? 4f : 0.5f) * speed * move.normalized;
        rb.AddForce(move, ForceMode.Acceleration);
    }
    private void Jump()
    {
        if (isGrounded && isJumped)
        {
            rb.AddForce(jumpStrength * Vector3.up, ForceMode.Impulse);
        }
    }
}