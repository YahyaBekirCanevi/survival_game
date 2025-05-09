using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float airControlMultiplier = 0.5f;
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float jumpTime = 0.3f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float coyoteTime = 0.2f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private Vector3 inputDir;
    private bool isGrounded;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float jumpTimeCounter;
    private bool isJumping;
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (groundCheck == null)
        {
            groundCheck = new GameObject("GroundCheck").transform;
            groundCheck.parent = transform;
            groundCheck.localPosition = Vector3.down * 0.01f;
        }
    }

    private void Update()
    {
        inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;

        rb.drag = isGrounded ? groundDrag : airDrag;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        HandleMovement();
        HandleJump();
        ApplyJumpPhysics();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        coyoteTimeCounter = isGrounded ? coyoteTime : coyoteTimeCounter - Time.fixedDeltaTime;
        jumpBufferCounter -= Time.fixedDeltaTime;
    }

    private void HandleMovement()
    {
        Vector3 move = transform.TransformDirection(inputDir) * (isGrounded ? moveSpeed : moveSpeed * airControlMultiplier);
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        currentSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
    }

    private void HandleJump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            // rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpTimeCounter = jumpTime;
            isJumping = true;
            jumpBufferCounter = 0;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping && jumpTimeCounter > 0)
        {
            rb.AddForce(Vector3.up * jumpForce * 0.5f, ForceMode.Force);
            jumpTimeCounter -= Time.fixedDeltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space))
            isJumping = false;
    }

    private void ApplyJumpPhysics()
    {
        if (rb.velocity.y < 0)
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
    }

    public bool IsGrounded => isGrounded;
    public float CurrentSpeed => currentSpeed;
    public Vector3 MoveDirection => inputDir;

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
