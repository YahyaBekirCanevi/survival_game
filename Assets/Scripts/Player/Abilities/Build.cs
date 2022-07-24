using UnityEngine;
using UnityEngine.UI;

public class Build : MonoBehaviour
{
    [SerializeField] private GameObject[] objs;
    [SerializeField] private float distanceBetween;
    [SerializeField] private ItemObject item;
    [SerializeField][ColorUsage(true, true)] private Color correctPlacement, falsePlacement;
    [SerializeField] private LayerMask build;
    [SerializeField] private Material ghostMaterial;
    [SerializeField] private Camera _camera;
    [SerializeField] private Text openDoor;
    private Material actualMaterial;
    private GameObject ghost = null, picked = null;
    private float rot = 0, scroll = 0;
    private float time = 0, rotationSpeed = 60, heigth = 0;
    private int index = 0;
    private bool rePos = false, released = false, correctPlace = true;
    private Ray ray;
    private void Awake()
    {
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && released)
        {
            index = (index + 1) % objs.Length;
            if (ghost != null)
            {
                Vector3 position = ghost.transform.position;
                Quaternion rotation = ghost.transform.rotation;
                DestroyObj(ref ghost);
                CreateObj();
                ghost.transform.position = position;
                ghost.transform.rotation = rotation;
            }
        }
        ray = _camera.ViewportPointToRay(new Vector3(.5f, .5f, _camera.nearClipPlane));
        if (picked == null) Building();
        if (ghost == null) RePosition();
    }
    private void RePosition()
    {
        if (rePos)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0)) released = true;
            MoveObj(ref picked);
            return;
        }
        float wait = 1;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 30, build))
                ResetPick(hit.collider.gameObject);
            else
                ResetPick(null);
        }
        else if (Input.GetKey(KeyCode.Mouse0) && Time.time - time > wait)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 30, build))
            {
                if (hit.collider.gameObject != picked)
                    ResetPick(hit.collider.gameObject);
                else
                {
                    rePos = true;
                    InitObj(ref picked);
                }
            }
            else
                ResetPick(null);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && picked != null && Time.time - time < wait)
            ResetPick(null);
    }
    private void Building()
    {
        if (Input.GetKeyDown(KeyCode.C)) CreateObj();
        if (ghost != null)
        {
            released = true;
            MoveObj(ref ghost);
        }
    }
    private void MoveObj(ref GameObject go)
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            DestroyObj(ref go);
            rePos = false;
            return;
        }
        heigth += (Input.GetKeyDown(KeyCode.Q) ? 1 : 0) + (Input.GetKeyDown(KeyCode.E) ? -1 : 0);
        if (Physics.Raycast(ray, out RaycastHit hit, 30))
        {
            if (!go.activeInHierarchy) go.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Mouse0) && released && correctPlace)
            {
                PlaceObj(ref go);
                return;
            }
            Collider collider = null;
            if (hit.collider.gameObject.CompareLayer(build)) collider = hit.collider;
            else
            {
                Collider[] colliders = Physics.OverlapSphere(hit.point, distanceBetween / 2, build);
                collider = colliders.Length > 0 ? colliders[0] : null;
            }
            if (collider && go != collider.gameObject)
            {
                Vector3 direction = hit.point;
                direction.y = collider.gameObject.transform.position.y;
                direction -= collider.gameObject.transform.position;
                SnapObj(ref go, direction, collider);
            }
            else if (hit.collider.gameObject.CompareTag("Ground"))
            {
                correctPlace = true;
                go.transform.position = hit.point + go.transform.up * Mathf.Abs(
                    go.GetComponent<MeshFilter>().mesh.bounds.size.y) + Vector3.up * heigth;
                RotateObj(ref go, Input.GetAxis("Mouse ScrollWheel") * rotationSpeed);
            }
            else
                go.SetActive(false);
            go.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor",
                correctPlace ? correctPlacement : falsePlacement);
        }
        else
            go.SetActive(false);
    }
    private void SnapObj(ref GameObject go, Vector3 direction, Collider collider)
    {
        if (!collider.CompareTag("Roof"))
        {
            float yRotation = Quaternion.LookRotation(direction).eulerAngles.y;
            rot = ((int)((yRotation % 360) / 90) - 1) * 90;
            float _scroll = Input.GetAxis("Mouse ScrollWheel");
            scroll += _scroll != 0 ? (_scroll > 0 ? 1 : -1) : 0;
            scroll %= 4;
            Vector3 pos = Vector3.zero;
            if (collider.CompareTag("Ground"))
            {
                pos = (go.CompareTag("Ground") && !go.name.Contains("Stair")) ? SnapXY(direction) : Vector3.zero;
                pos.y = 0;
            }
            else
                pos = Vector3.up * distanceBetween;

            if (pos != Vector3.zero)
                correctPlace = !Physics.Raycast(collider.transform.position, pos, distanceBetween, build);

            go.transform.position = collider.transform.position + pos + Vector3.up * heigth;
            go.transform.eulerAngles = new Vector3(-90, rot + (scroll * 90), 0);
        }
        else
        {
            correctPlace = false;
        }
    }
    private Vector3 SnapXY(Vector3 euler)
    {
        Quaternion rotation = Quaternion.Euler(euler);
        return Mathf.Abs(euler.x) > Mathf.Abs(euler.z) ?
            (distanceBetween * (euler.x / Mathf.Abs(euler.x))) * (rotation * Vector3.right)
            : (distanceBetween * (euler.z / Mathf.Abs(euler.z))) * (rotation * Vector3.forward);
    }
    private void ResetPick(GameObject go)
    {
        time = Time.time;
        picked = go;
    }
    public void CreateObj()
    {
        if (ghost != null)
        {
            print("Another instance of ghost already occurs!");
            return;
        }
        ghost = Instantiate(objs[index], GameObject.Find("Environment").transform.Find("Builds").transform);
        if (ghost.transform.childCount > 0)
            if (ghost.transform.GetChild(0).TryGetComponent<Door>(out Door door)) door.text = openDoor;
        InitObj(ref ghost);
    }
    private void InitObj(ref GameObject go)
    {
        if (go.GetComponent<Collider>())
            go.GetComponent<Collider>().enabled = false;
        foreach (var c in go.GetComponentsInChildren<Collider>())
            c.enabled = false;
        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
        actualMaterial = renderer.material;
        renderer.material = ghostMaterial;
        renderer.material.SetColor("_EmissionColor", falsePlacement);
        released = false;
        go.SetActive(false);
    }
    private void PlaceObj(ref GameObject go)
    {
        if (go.GetComponent<Collider>())
            go.GetComponent<Collider>().enabled = true;
        foreach (var c in go.GetComponentsInChildren<Collider>())
            c.enabled = true;
        go.GetComponent<MeshRenderer>().material = actualMaterial;
        actualMaterial = default(Material);
        go = null;
        released = rePos = false;
        heigth = 0;
        scroll = 0;
    }
    private void DestroyObj(ref GameObject go)
    {
        Destroy(go);
        go = null;
    }
    private void RotateObj(ref GameObject go, float rotation) => go.transform.Rotate(Vector3.forward, rotation);
    private void OnDrawGizmos()
    {
        if (!correctPlace)
            if (Physics.Raycast(ray, out RaycastHit hit, 30) && ghost != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(ghost.GetComponent<Collider>() ? ghost.GetComponent<Collider>().bounds.center :
                   ghost.GetComponentInChildren<Collider>().bounds.center, .1f);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(hit.collider.transform.position + Vector3.up * 1, .1f);
            }
    }
}