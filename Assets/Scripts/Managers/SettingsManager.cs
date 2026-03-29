using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq; //filtrare

public class SettingsManager : MonoBehaviour
{
    public GameObject panelSettings;
    public GameObject panelMeniu;

    public Toggle toggleSunet;
    public Toggle toggleFullscreen;

    public TextMeshProUGUI textVolum;
    public Slider sliderVolum;

    public Slider sliderBright;
    public UnityEngine.UI.Image panelBright;

    public TMP_Dropdown dropdownRezolutie;

    private float volumSalvat = 1f;

    void Start()
    {
        PopuleazaRezolutii();
        IncarcaSetari();
    }

    void IncarcaSetari()
    {
        // volum
        volumSalvat = PlayerPrefs.GetFloat("Volum", 1f);
        bool sunetActiv = PlayerPrefs.GetInt("Sunet", 1) == 1;
        toggleSunet.SetIsOnWithoutNotify(sunetActiv);
        sliderVolum.value = sunetActiv ? volumSalvat : 0f;
        sliderVolum.interactable = sunetActiv;
        AudioListener.volume = sunetActiv ? volumSalvat : 0f;
        UpdateTextVolum(sliderVolum.value);

        // fullscreen
        bool isFS = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        toggleFullscreen.SetIsOnWithoutNotify(isFS);
        SchimbaFullscreen(isFS);

        // luminozitate
        float bright = PlayerPrefs.GetFloat("Luminozitate", 1f);
        sliderBright.value = bright;
        AplicaBright(bright);
    }

    public void DeschideSettings()
    {
        panelMeniu.SetActive(false);
        panelSettings.SetActive(true);
    }

    public void InchideSettings()
    {
        panelSettings.SetActive(false);
        panelMeniu.SetActive(true);
    }

    public void AplicaVolum(float valoare)
    {
        if (toggleSunet.isOn)
        {
            AudioListener.volume = valoare;
            volumSalvat = valoare; // salvam mereu ultima valoare trasa
        }
        PlayerPrefs.SetFloat("Volum", valoare);
        if (textVolum != null)
            textVolum.text = Mathf.RoundToInt(valoare * 100) + "%"; //procentul de langa text
    }

    public void AplicaSunet(bool activ)
    {
        if (activ)
        {
            // volum salvat
            sliderVolum.value = volumSalvat;
            AudioListener.volume = volumSalvat;
        }
        else
        {
            // cazul in care dai disable si ai deja volum, il salvam
            volumSalvat = sliderVolum.value > 0 ? sliderVolum.value : volumSalvat;
            sliderVolum.value = 0f;
            AudioListener.volume = 0f;
        }

        sliderVolum.interactable = activ;
        PlayerPrefs.SetInt("Sunet", activ ? 1 : 0);

        if (textVolum != null)
            textVolum.text = Mathf.RoundToInt(sliderVolum.value * 100) + "%";
    }

    void PopuleazaRezolutii()
    {
        dropdownRezolutie.ClearOptions();

        // 4 rez uzuale
        List<string> optiuni = new List<string>
        {
            "1920 x 1080",
            "1600 x 900",
            "1366 x 768",
            "1280 x 720"
        };

        dropdownRezolutie.AddOptions(optiuni);

        // ce am salvat(implicit 1920x1080)
        int indexSalvat = PlayerPrefs.GetInt("Rezolutie", 0);

        // out of bounds
        if (indexSalvat >= optiuni.Count) indexSalvat = 0;

        dropdownRezolutie.value = indexSalvat;
        dropdownRezolutie.RefreshShownValue();
    }

    public void SchimbaRezolutie(int index)
    {
        // extragem ce am ales
        string[] dimensiuni = dropdownRezolutie.options[index].text.Split('x');
        int w = int.Parse(dimensiuni[0].Trim());
        int h = int.Parse(dimensiuni[1].Trim());

        //aplicam
        Screen.SetResolution(w, h, Screen.fullScreen);

        PlayerPrefs.SetInt("Rezolutie", index);
    }

    public void SchimbaFullscreen(bool activ)
    {
        // bug de culori
        Screen.fullScreenMode = activ ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.fullScreen = activ;

        //
        int index = PlayerPrefs.GetInt("Rezolutie", 0);
        SchimbaRezolutie(index);

        PlayerPrefs.SetInt("Fullscreen", activ ? 1 : 0);
    }

    public void AplicaBright(float valoare)
    {
        //val 1 maxim, val 0 minim
        float a = 1f - valoare;
        panelBright.color = new Color(0, 0, 0, a * 0.8f); // 0.8 limitare să nu fie beznă totală
        PlayerPrefs.SetFloat("Luminozitate", valoare);
    }

    void UpdateTextVolum(float v)
    {
        if (textVolum != null) textVolum.text = Mathf.RoundToInt(v * 100) + "%";
    }
}