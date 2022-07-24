using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    public Text text;
    [SerializeField] private Transform doorHandle;
    [SerializeField] private LayerMask build;
    [SerializeField] private float minAngle = 0, maxAngle = 90;
    [SerializeField] private float minDistance = 1;
    [SerializeField] private bool open = false;
    private float angle = 0;
    private CameraController cam;
    private void Awake()
    {
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
        TextEnable(cam.Cast(3, build));
        Physics.Raycast(doorHandle.position + Vector3.up * .5f, -doorHandle.up, out RaycastHit upHit, minDistance);
        Physics.Raycast(doorHandle.position, -doorHandle.up, out RaycastHit midHit, minDistance);
        Physics.Raycast(doorHandle.position - Vector3.up * .4f, -doorHandle.up, out RaycastHit lowHit, minDistance);
        if ((!upHit.collider && !midHit.collider && !lowHit.collider) || !open)
        {
            angle = Mathf.Lerp(angle, open ? maxAngle : minAngle, .05f);
            transform.rotation = Quaternion.Euler(transform.parent.eulerAngles + new Vector3(0, angle, 0));
        }
    }
    private void TextEnable(Collider col)
    {
        if (col)
        {
            if (col.gameObject == transform.gameObject)
            {
                text.enabled = true;
                if (Input.GetKeyDown(KeyCode.F))
                    open = !open;
            }
            else
                text.enabled = false;
        }
        else
            text.enabled = false;
    }
}