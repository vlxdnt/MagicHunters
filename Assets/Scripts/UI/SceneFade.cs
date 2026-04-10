using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; //schimbari de scena

// intre scene
// merge tot pe dontdestroyonload ca sa continue
public class SceneFade : MonoBehaviour
{
    public static SceneFade Instance;
    public Image panelFade;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 

            if (panelFade != null)
            {
                Color c = panelFade.color;
                c.a = 1f; // pt fade out calumea la prima pornire
                panelFade.color = c;
                panelFade.gameObject.SetActive(true);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            if (panelFade != null)
            {
                panelFade.gameObject.SetActive(true);
                panelFade.color = new Color(0, 0, 0, 1f);
            }
        }
        else
        {
            if (panelFade != null)
            {
                panelFade.color = new Color(0, 0, 0, 1f);
                panelFade.gameObject.SetActive(true);

                StartCoroutine(FadeIn(1f));
            }
        }
    }

    // fade in propriu zis 
    public IEnumerator FadeIn(float durata = 0.5f)
    {
        float t = 0;
        panelFade.gameObject.SetActive(true);

        while (t < durata)
        {
            t += Time.deltaTime;
            panelFade.color = new Color(0, 0, 0, 1f - (t / durata));
            yield return null; // sa astepte frame ul anterior
        }
        panelFade.color = new Color(0, 0, 0, 0);
        panelFade.gameObject.SetActive(false);
    }

    // fade out (face ecranul negru)
    public IEnumerator FadeOut(float durata = 1.5f)
    {
        float t = 0;
        panelFade.gameObject.SetActive(true);
        panelFade.color = new Color(0, 0, 0, 0);

        while (t < durata)
        {
            t += Time.deltaTime;
            panelFade.color = new Color(0, 0, 0, (t / durata));
            yield return null; // sa astepte frame ul anterior
        }
        panelFade.color = new Color(0, 0, 0, 1);
    }
}