using UnityEngine;
using TMPro;

public class TitluAnimat : MonoBehaviour
{
    [Header("Zoom")]
    public float scalaMinima = 0.95f;
    public float scalaMaxima = 1.05f;
    public float vitezaZoom = 2f;

    [Header("Rotatie")]
    public float unghiMaxim = 3f; // grade
    public float vitezaRotatie = 1.5f;

    private Vector3 scalaOriginala;

    void Start()
    {
        scalaOriginala = transform.localScale;
    }

    void Update()
    {
        // Zoom in/out cu Mathf.Sin pentru miscare lina
        float scala = Mathf.Lerp(scalaMinima, scalaMaxima, (Mathf.Sin(Time.time * vitezaZoom) + 1f) / 2f);
        transform.localScale = scalaOriginala * scala;

        // Rotatie stanga/dreapta
        float rotatie = Mathf.Sin(Time.time * vitezaRotatie) * unghiMaxim;
        transform.rotation = Quaternion.Euler(0f, 0f, rotatie);
    }
}