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
    public Toggle toggleMuzica;

    public TextMeshProUGUI textVolum;
    public Slider sliderVolum;

    public Slider sliderBright;
    public UnityEngine.UI.Image panelBright;

    public TMP_Dropdown dropdownRezolutie;

    private float volumSalvat = 1f;

    void Start()
    {
        if (dropdownRezolutie != null)
            PopuleazaRezolutii();

        IncarcaSetari();
    }

    public void IncarcaSetari()
    {
        // volum
        volumSalvat = PlayerPrefs.GetFloat("Volum", 1f);
        bool sunetActiv = PlayerPrefs.GetInt("Sunet", 1) == 1;

        if (sliderVolum != null)
        {
            sliderVolum.value = sunetActiv ? volumSalvat : 0f;
            sliderVolum.interactable = sunetActiv;
        }
        if (toggleSunet != null)
            toggleSunet.SetIsOnWithoutNotify(sunetActiv);

        AudioListener.volume = sunetActiv ? volumSalvat : 0f;
        UpdateTextVolum(sunetActiv ? volumSalvat : 0f);

        // muzica
        bool muzicaActiva = PlayerPrefs.GetInt("MuzicaActiva", 1) == 1;
        if (toggleMuzica != null)
            toggleMuzica.SetIsOnWithoutNotify(muzicaActiva);
        if (BackgroundMusic.instance != null)
            BackgroundMusic.instance.SeteazaMuteMuzica(muzicaActiva);

        // doar in MainMenu
        if (toggleFullscreen != null)
        {
            bool isFS = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            toggleFullscreen.SetIsOnWithoutNotify(isFS);
            SchimbaFullscreen(isFS);
        }
        if (sliderBright != null && panelBright != null)
        {
            float bright = PlayerPrefs.GetFloat("Luminozitate", 1f);
            sliderBright.value = bright;
            AplicaBright(bright);
        }
        if (dropdownRezolutie != null)
            PopuleazaRezolutii();
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
        if (toggleSunet == null || toggleSunet.isOn)
        {
            AudioListener.volume = valoare;
            volumSalvat = valoare;
        }
        PlayerPrefs.SetFloat("Volum", valoare);
        if (textVolum != null)
            textVolum.text = Mathf.RoundToInt(valoare * 100) + "%";
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

    public void AplicaMuzica(bool activ)
    {
        if (BackgroundMusic.instance != null)
        {
            BackgroundMusic.instance.SeteazaMuteMuzica(activ);
        }
        PlayerPrefs.SetInt("MuzicaActiva", activ ? 1 : 0);
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