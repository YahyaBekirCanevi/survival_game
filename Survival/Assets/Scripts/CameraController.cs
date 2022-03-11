using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool availableToShake;
    private float amountOfShake;
    private float shakeMultiplier;
    private float perlinScale;
    private float shakeDuration;
    private float timeCount = 0;
    public bool zoom { get; set; }
    [SerializeField] private Camera main, weapon;
    void Update()
    {
        if (availableToShake && timeCount < shakeDuration)
        {
            timeCount += Time.deltaTime;
            transform.localPosition = (Vector3.up * 2) + GetNoiseVector() * (shakeDuration - timeCount) / shakeDuration;
        }
        else
        {
            timeCount = 0;
            availableToShake = false;
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.up * 2, Time.deltaTime);
        }
        zoom = Input.GetKey(KeyCode.Mouse1);
    }
    public void Zoom()
    {
        main.fieldOfView = Mathf.Lerp(main.fieldOfView, 35, .4f);
        weapon.fieldOfView = Mathf.Lerp(weapon.fieldOfView, 45, .4f);
    }
    public void UnZoom()
    {
        main.fieldOfView = Mathf.Lerp(main.fieldOfView, 70, .4f);
        weapon.fieldOfView = Mathf.Lerp(weapon.fieldOfView, 60, .4f);
    }
    private float GetPerlin(float seed) => (Mathf.PerlinNoise(seed, timeCount * Mathf.Pow(amountOfShake, shakeMultiplier)) - .5f) * 2;
    private Vector3 GetNoiseVector() => new Vector3(GetPerlin(1), GetPerlin(10), 0) * perlinScale;
    public void ShakeLow()
    {
        availableToShake = true;
        amountOfShake = .25f;
        shakeMultiplier = .2f;
        perlinScale = .25f;
        shakeDuration = 2f;
        timeCount = 0;
    }
    public void ShakeHigh()
    {
        availableToShake = true;
        amountOfShake = .01f;
        shakeMultiplier = -.75f;
        perlinScale = .5f;
        shakeDuration = .25f;
        timeCount = 0;
    }
    public Collider Cast(float range, LayerMask layer, float punchThrough = 1) => CastHit(range, layer, punchThrough).collider;
    public RaycastHit CastHit(float range, LayerMask layer, float punchThrough = 1)
    {
        Ray ray = main.ViewportPointToRay(new Vector3(.5f, .5f, main.nearClipPlane));
        ///Debug.DrawRay(transform.position, transform.forward * range, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            if (Physics.Raycast(ray, out RaycastHit hit2, range, layer))
                if (Vector3.Distance(hit.point, hit2.point) <= punchThrough)
                    return hit2;
                else return default(RaycastHit);
            else return default(RaycastHit);
        }
        else return default(RaycastHit);
    }
}