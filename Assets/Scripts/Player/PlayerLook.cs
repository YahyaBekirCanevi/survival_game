using UnityEngine;
using System;

public class PlayerLook : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 1.5f;
    [SerializeField] private float maxLookAngle = 60f;
    [SerializeField] private float minLookAngle = -60f;
    [SerializeField] private float cameraSmoothTime = 0.1f;
    [SerializeField] private float cameraCollisionRadius = 0.2f;
    [SerializeField] private float cameraCollisionOffset = 0.2f;
    [SerializeField] private LayerMask cameraCollisionLayers;

    [Header("References")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform cameraHolder;

    // State
    private float currentRotationX;
    private float currentRotationY;
    private Vector3 currentRotation;
    private Vector3 smoothVelocity = Vector3.zero;
    private bool isLocked = true;

    // Events
    public event Action<bool> OnLockStateChanged;

    private void Awake()
    {
        if (cameraController == null)
            cameraController = FindObjectOfType<CameraController>();
        
        if (cameraHolder == null)
            cameraHolder = cameraController.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        enabled = newGameState == GameState.Gameplay;
    }

    private void Update()
    {
        if (!isLocked) return;

        HandleInput();
        UpdateCameraRotation();
        HandleCameraCollision();
    }

    private void HandleInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentRotationX += mouseX;
        currentRotationY -= mouseY;
        currentRotationY = Mathf.Clamp(currentRotationY, minLookAngle, maxLookAngle);
    }

    private void UpdateCameraRotation()
    {
        // Smoothly rotate the player
        Vector3 targetPlayerRotation = new Vector3(0f, currentRotationX, 0f);
        transform.rotation = Quaternion.Euler(targetPlayerRotation);

        // Smoothly rotate the camera
        Vector3 targetCameraRotation = new Vector3(currentRotationY, 0f, 0f);
        cameraHolder.localRotation = Quaternion.Euler(targetCameraRotation);
    }

    private void HandleCameraCollision()
    {
        Vector3 targetPosition = cameraHolder.position;
        Vector3 direction = cameraHolder.position - transform.position;
        float distance = direction.magnitude;

        // Check for camera collision
        if (Physics.SphereCast(transform.position, cameraCollisionRadius, direction.normalized, 
            out RaycastHit hit, distance, cameraCollisionLayers))
        {
            targetPosition = hit.point + (hit.normal * cameraCollisionOffset);
        }

        // Smoothly move camera to target position
        cameraHolder.position = Vector3.SmoothDamp(
            cameraHolder.position,
            targetPosition,
            ref smoothVelocity,
            cameraSmoothTime
        );
    }

    public void SetLockState(bool locked)
    {
        isLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
        OnLockStateChanged?.Invoke(locked);
    }

    public void SetSensitivity(float sensitivity)
    {
        mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 5f);
    }
}
