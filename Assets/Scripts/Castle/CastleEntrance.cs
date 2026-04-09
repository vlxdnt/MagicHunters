using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class CastleEntrance : MonoBehaviour
{
    [Header("Exterior")]
    public GameObject castleExt;
    public GameObject usa;

    [Header("Interior")]
    public GameObject castleInt;
    public GameObject ziduriInvizibile;

    [Header("Fade")]
    public float fadeDuration = 1.5f;

    [Header("Limite cameră interior")]
    public float xMinInterior = 10f;
    public float xMaxInterior = 30f;
    public float yMinInterior = -5f;
    public float yMaxInterior = 10f;

    private int jucatoriInZona = 0;
    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            jucatoriInZona++;
            // ambii playeri
            if (jucatoriInZona >= 2)
            {
                triggered = true;
                StartCoroutine(IntrareInCastel());
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jucatoriInZona = Mathf.Max(0, jucatoriInZona - 1);
    }

    IEnumerator IntrareInCastel()
    {
        // fade out pt exterior
        Tilemap[] tilemaps = castleExt.GetComponentsInChildren<Tilemap>();
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            foreach (Tilemap t in tilemaps)
            {
                Color c = t.color;
                c.a = alpha;
                t.color = c;
            }
            yield return null;
        }

        // dezac exerior+usa
        castleExt.SetActive(false);
        if (usa != null) usa.SetActive(false);

        // activ int
        castleInt.SetActive(true);
        ziduriInvizibile.SetActive(true);

        // limitele camerelor
        CameraLimits[] camere = FindObjectsByType<CameraLimits>(FindObjectsSortMode.None);
        foreach (CameraLimits cam in camere)
        {
            cam.xMin = xMinInterior;
            cam.xMax = xMaxInterior;
            cam.yMin = yMinInterior;
            cam.yMax = yMaxInterior;
        }
    }
}