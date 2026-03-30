using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//intre scene
//merge tot pe dontdestroyonload ca sa continue
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
        }
        else Destroy(gameObject);
    }

    //fade in propriu zis
    public IEnumerator FadeIn(float durata = 0.5f)
    {
        float t = 0;
        panelFade.gameObject.SetActive(true);

        while(t < durata)
        {
            t += Time.deltaTime;
            panelFade.color = new Color(0, 0, 0, 1f - (t / durata));
            yield return null; // sa astepte frame ul anterior
        }
        panelFade.color = new Color(0, 0, 0, 0);
        panelFade.gameObject.SetActive(false);
    }

    //fade out
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
