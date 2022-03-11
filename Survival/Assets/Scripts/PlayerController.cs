using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private float speed;
    [SerializeField] private float walkSpeed = 8;
    [SerializeField] private float lowSpeed = 4;
    [SerializeField] private float runSpeed = 12;
    [SerializeField] private float jumpStrength = 8;
    [SerializeField] [Range(.1f, 2)] private float mouseSensitivity = 1.5f;
    private float xRot = 0, yRot = 0;
    private float yScale;
    private CapsuleCollider cc;
    private Inventory inventory;
    private Rigidbody rb;
    private CameraController cam;
    private Transform body;
    private float GroundDistance
    {
        get => Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 30) ? hit.distance : 30;
    }
    private bool isGrounded;
    public bool IsGrounded { get => isGrounded; }
    private bool Grounded
    {
        get => GroundDistance <= 1.5f;
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        cam = GameObject.FindObjectOfType<CameraController>();
        yScale = cc.height;
        body = GetComponentInChildren<MeshRenderer>().transform;
        inventory = GetComponent<Inventory>();
        xRot = transform.rotation.eulerAngles.y;
        yRot = transform.rotation.eulerAngles.x;
    }
    void Update()
    {
        isGrounded = Grounded;
        CalculateSpeed();
        Move();
        Look();
        if (Input.GetKeyDown(KeyCode.I))
            inventory.PrintCountOfAllObjects();
    }
    private void CalculateSpeed()
    {
        if (IsGrounded)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                speed = lowSpeed;
                cc.height = yScale * .5f;
            }
            else
            {
                cc.height = yScale;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    speed = runSpeed;
                }
                else
                {
                    speed = walkSpeed;
                }
            }
        }
        else
        {
            cc.height = yScale;
            speed = walkSpeed;
        }
        body.localScale = Vector3.one * cc.height * (1 / 2);
    }
    private void Move()
    {
        float ver = Input.GetAxisRaw("Vertical");
        float hor = Input.GetAxisRaw("Horizontal");
        Vector3 move = cam.transform.forward * ver + cam.transform.right * hor;
        move.y = 0;
        move = move.normalized * speed;
        Jump();
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
    }
    private void Jump()
    {
        if (IsGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpStrength * Vector3.up, ForceMode.Impulse);
        }
    }
    private void Look()
    {
        xRot += Input.GetAxis("Mouse X") * mouseSensitivity;
        yRot -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        yRot = Mathf.Clamp(yRot, -60, 60);
        transform.rotation = Quaternion.Euler(0, xRot, 0);
        cam.transform.rotation = Quaternion.Euler(yRot, xRot, 0);
    }
}