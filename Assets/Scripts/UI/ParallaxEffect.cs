using UnityEngine;

public class ParallaxInfinit : MonoBehaviour
{
    private float lungime, startPos;
    private Transform cam;

    [Header("Setari")]
    [Range(0, 1)]
    public float efectParallax = 0.4f;

    void Start()
    {
        startPos = transform.position.x;
        lungime = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        if (cam == null)
        {
            if (Camera.main != null) cam = Camera.main.transform;
            return;
        }

        // poz relativa
        float temp = (cam.position.x * (1 - efectParallax));
        float dist = (cam.position.x * efectParallax);

        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        // repetarea infinita
        if (temp > startPos + lungime) startPos += lungime;
        else if (temp < startPos - lungime) startPos -= lungime;
    }
}