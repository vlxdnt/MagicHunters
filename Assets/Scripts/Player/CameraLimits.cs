using UnityEngine;

public class CameraLimits : MonoBehaviour
{
    [Header("Limite")]
    public float xMin = 0f;
    public float xMax = 50f;
    public float yMin = -5f;
    public float yMax = 10f;

    [Header("Offset pentru camera")]
    public float offsetX = 0f;
    public float offsetY = 1.5f;

    [Header("Smoothing")]
    public float smoothSpeed = 5f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = GetComponentInChildren<Camera>();
    }

    public void SetLimits(float nouXMin, float nouXMax, float nouYMin, float nouYMax)
    {
        xMin = nouXMin;
        xMax = nouXMax;
        yMin = nouYMin;
        yMax = nouYMax;
    }

    void LateUpdate()
    {
        if (cam == null) return;
        if (transform.parent == null) return;

        float vertExtent = cam.orthographicSize;
        float horizExtent = vertExtent * cam.aspect;

        // urm parintele
        Vector3 targetPos = transform.parent.position;
        targetPos.x += offsetX;
        targetPos.y += offsetY;
        targetPos.z = -10f;

        float leftBound = xMin + horizExtent;
        float rightBound = xMax - horizExtent;
        float bottomBound = yMin + vertExtent;
        float topBound = yMax - vertExtent;

        targetPos.x = (leftBound < rightBound) ? Mathf.Clamp(targetPos.x, leftBound, rightBound) : (xMin + xMax) / 2f;
        targetPos.y = (bottomBound < topBound) ? Mathf.Clamp(targetPos.y, bottomBound, topBound) : (yMin + yMax) / 2f;

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}