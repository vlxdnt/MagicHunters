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
    public float smoothSpeed = 5f; // pentru delay fct

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = GetComponentInChildren<Camera>();
    }

    void LateUpdate()
    {
        if (cam == null) return;
        if (transform.parent == null) return;

        float vertExtent = cam.orthographicSize;
        float horizExtent = vertExtent * cam.aspect;

        // pozitia tinta 
        Vector3 targetPos = transform.parent.position;
        targetPos.x += offsetX;
        targetPos.y += offsetY;
        targetPos.z = -10f;

        // clamping la limite
        targetPos.x = Mathf.Clamp(targetPos.x, xMin + horizExtent, xMax - horizExtent);
        targetPos.y = Mathf.Clamp(targetPos.y, yMin + vertExtent, yMax - vertExtent);

        // miscare lina
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}