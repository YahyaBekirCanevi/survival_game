using UnityEngine;

enum MovementType { Idle, Walk, Run, Crouch, Jump }

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private float yScale;
    [ShowOnly, SerializeField] private float speed;
    [ShowOnly, SerializeField] private Vector3 move;
    [ShowOnly, SerializeField] private MovementType movementType;

    [Header("Movement")]
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
    private float GroundDistance
    {
        get => Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 30) ? hit.distance : 30;
    }
    [SerializeReference] private bool isGrounded, isCrouching, isRunning;
    public bool IsGrounded { get => isGrounded; }
    public float MovementSpeed { get => move.normalized.magnitude; }
    private bool Grounded
    {
        get => GroundDistance <= 1.25f;
    }
    void Awake()
    {
        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        yScale = cc.height;
        movementType = MovementType.Idle;
        rb.freezeRotation = true;
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
        isRunning = Input.GetKey(KeyCode.LeftShift);
    }

    private void ControlDrag()
    {
        rb.drag = isGrounded ? groundDrag : airDrag;
    }

    void FixedUpdate()
    {
        isGrounded = Grounded;

        speed = CalculateSpeed();

        Move();
        Jump();

        cc.height = movementType == MovementType.Crouch ? yScale * 0.5f : yScale;
        body.localScale = Vector3.one * cc.height;
    }
    private float CalculateSpeed()
    {
        movementType = isGrounded
            ? isCrouching
                ? MovementType.Crouch
                : isRunning
                    ? MovementType.Run
                    : move.magnitude > 0
                        ? MovementType.Walk
                        : MovementType.Idle
            : MovementType.Jump;



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
        move = move.normalized * speed * (isGrounded ? 4f : 0.5f);
        rb.AddForce(move, ForceMode.Acceleration);
    }
    private void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpStrength * Vector3.up, ForceMode.Impulse);
        }
    }
}