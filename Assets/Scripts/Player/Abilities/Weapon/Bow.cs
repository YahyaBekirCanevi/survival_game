using UnityEngine;

public class Bow : Weapon
{
    [Header("Bow Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float maxDrawTime = 2f;
    [SerializeField] private float minArrowSpeed = 20f;
    [SerializeField] private float maxArrowSpeed = 40f;
    [SerializeField] private float zoomFOV = 30f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float maxPullbackDistance = 0.5f; // How far back the arrow can be pulled

    private Arrow currentArrow;
    private float drawTime;
    private bool isDrawing;
    private bool isZoomed;
    private bool released = true;

    protected override void Awake()
    {
        base.Awake();
        if (arrowSpawnPoint == null)
        {
            arrowSpawnPoint = transform;
        }
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

    protected override void Update()
    {
        base.Update();

        // Handle bow drawing
        if (Input.GetKeyDown(KeyCode.Mouse0) && currentArrow == null)
        {
            StartDrawing();
        }
        else if (Input.GetKey(KeyCode.Mouse0) && isDrawing)
        {
            ContinueDrawing();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && isDrawing)
        {
            ReleaseArrow();
        }

        // Handle zoom
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartZoom();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopZoom();
        }

        // Update animations
        UpdateAnimations();
    }

    private void StartDrawing()
    {
        isDrawing = true;
        released = false;
        drawTime = 0f;
        SpawnArrow();
    }

    private void ContinueDrawing()
    {
        drawTime = Mathf.Min(drawTime + Time.deltaTime, maxDrawTime);
        
        // Update arrow position and rotation
        if (currentArrow != null)
        {
            float drawProgress = drawTime / maxDrawTime;
            // Pull back the arrow based on draw progress
            Vector3 pullbackOffset = -transform.forward * (maxPullbackDistance * drawProgress);
            currentArrow.transform.position = arrowSpawnPoint.position + pullbackOffset;
            currentArrow.transform.rotation = arrowSpawnPoint.rotation;
        }
    }

    private void ReleaseArrow()
    {
        if (currentArrow != null)
        {
            float drawProgress = drawTime / maxDrawTime;
            float arrowSpeed = Mathf.Lerp(minArrowSpeed, maxArrowSpeed, drawProgress);
            currentArrow.Fire();
            currentArrow = null;
        }
        
        isDrawing = false;
        released = true;
        drawTime = 0f;
    }

    private void SpawnArrow()
    {
        GameObject arrowObj = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        currentArrow = arrowObj.GetComponent<Arrow>();
        currentArrow.Initialize(arrowSpawnPoint, minArrowSpeed);
    }

    private void StartZoom()
    {
        isZoomed = true;
        cam.SetFOV(zoomFOV);
    }

    private void StopZoom()
    {
        isZoomed = false;
        cam.SetFOV(normalFOV);
    }

    private void UpdateAnimations()
    {
        anim.SetBool("attack", isDrawing);
        anim.SetBool("release", released);
        anim.SetFloat("drawProgress", drawTime / maxDrawTime);
        anim.SetBool("isZoomed", isZoomed);
    }

    public override void Damage(float rate)
    {
        // Bow doesn't use the base damage system
        // Damage is handled by the Arrow script
    }
}