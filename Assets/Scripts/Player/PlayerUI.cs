using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.Netcode;

public class PlayerUI : NetworkBehaviour
{
    [Header("Referinte")]
    public GameObject panelPersonaj;
    public Image imgPersonaj;
    public Slider baraViata;
    public TextMeshProUGUI textJucator;

    [Header("Obiectiv")]
    public TextMeshProUGUI textObiectiv;
    public float fadeDuration = 0.5f;

    [Header("Sprites Personaje")]
    public Sprite spriteWitch;
    public Sprite spriteCat;

    public void ActualizeazaHP(float curent, float maxim)
    {
        if (baraViata != null)
        {
            baraViata.maxValue = maxim;
            baraViata.value = curent;
        }
    }

    public void SetObiectiv(string textNou)
    {
        if (textObiectiv != null)
            StartCoroutine(AnimatieObiectiv(textNou));
    }

    IEnumerator AnimatieObiectiv(string textNou)
    {
        // fade out
        float t = 0f;
        Color culoare = textObiectiv.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            culoare.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            textObiectiv.color = culoare;
            yield return null;
        }

        // shcimba textul
        textObiectiv.text = textNou;

        // fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            culoare.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            textObiectiv.color = culoare;
            yield return null;
        }
    }

    public void SetPersonaj(bool isWitch, ulong clientId)
    {

        if (imgPersonaj != null)
            imgPersonaj.sprite = isWitch ? spriteWitch : spriteCat;

        if (textJucator != null)
            textJucator.text = clientId == 0 ? "Player 1" : "Player 2";
        else
            Debug.Log("textJucator NULL");
    }

    void OnEnable()
    {
        Language.OnLimbaSchimbata += ActualizeazaObiectivLimba;
    }

    void OnDisable()
    {
        Language.OnLimbaSchimbata -= ActualizeazaObiectivLimba;
    }

    void ActualizeazaObiectivLimba()
    {
        if (ObjectiveManager.Instance != null)
            SetObiectiv(ObjectiveManager.Instance.GetObiectivCurent());
    }
}