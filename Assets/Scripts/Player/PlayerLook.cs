using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField][Range(.1f, 2)] private float mouseSensitivity = 1.5f;
    private float xRotation = 0, yRotation = 0;
    [SerializeField] private CameraController cam;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = GameObject.FindObjectOfType<CameraController>();
    }
    void Update()
    {
        Inputs();
        Look();
    }
    private void Inputs()
    {
        xRotation += Input.GetAxis("Mouse X") * mouseSensitivity;
        yRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        yRotation = Mathf.Clamp(yRotation, -60, 60);
    }
    private void Look()
    {
        transform.rotation = Quaternion.Euler(0, xRotation, 0);
        cam.transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
    }
}
