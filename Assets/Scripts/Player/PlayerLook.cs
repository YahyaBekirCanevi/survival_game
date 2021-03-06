using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    private bool isLocked = true;
    private float xRotation = 0, yRotation = 0;
    [SerializeField][Range(.1f, 2)] private float mouseSensitivity = 1.5f;
    [SerializeField] private CameraController cam;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = GameObject.FindObjectOfType<CameraController>();
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
        if (isLocked) Look();
    }
    private void Inputs()
    {
        if (isLocked)
        {
            xRotation += Input.GetAxis("Mouse X") * mouseSensitivity;
            yRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            yRotation = Mathf.Clamp(yRotation, -60, 60);
        }
    }
    private void Look()
    {
        transform.rotation = Quaternion.Euler(0, xRotation, 0);
        cam.transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
    }
}
