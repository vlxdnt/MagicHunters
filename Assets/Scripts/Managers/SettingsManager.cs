using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    public GameObject panelSettings;
    public GameObject panelMeniu;

    [Header("UI Toggles")]
    public Toggle toggleSunet;
    public Toggle toggleFullscreen;
    public Toggle toggleMuzica;

    [Header("UI Sliders & Text")]
    public TextMeshProUGUI textVolum;
    public Slider sliderVolum;
    public Slider sliderBright;
    public UnityEngine.UI.Image panelBright;

    [Header("UI Dropdown")]
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
        volumSalvat = PlayerPrefs.GetFloat("Volum", 1f);
        bool sunetActiv = PlayerPrefs.GetInt("Sunet", 1) == 1;

        if (sliderVolum != null)
        {
            sliderVolum.SetValueWithoutNotify(sunetActiv ? volumSalvat : 0f);
            sliderVolum.interactable = sunetActiv;
        }
        if (toggleSunet != null)
            toggleSunet.SetIsOnWithoutNotify(sunetActiv);

        AudioListener.volume = sunetActiv ? volumSalvat : 0f;
        UpdateTextVolum(sunetActiv ? volumSalvat : 0f);

        bool muzicaActiva = PlayerPrefs.GetInt("MuzicaActiva", 1) == 1;
        if (toggleMuzica != null)
            toggleMuzica.SetIsOnWithoutNotify(muzicaActiva);
        
        if (BackgroundMusic.instance != null)
            BackgroundMusic.instance.SeteazaMuteMuzica(muzicaActiva);

        if (sliderBright != null && panelBright != null)
        {
            float bright = PlayerPrefs.GetFloat("Luminozitate", 1f);
            sliderBright.SetValueWithoutNotify(bright);
            AplicaBright(bright);
        }

        bool isFS = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        int indexRezolutie = PlayerPrefs.GetInt("Rezolutie", 0);

        if (toggleFullscreen != null)
            toggleFullscreen.SetIsOnWithoutNotify(isFS);

        if (dropdownRezolutie != null)
        {
            if (indexRezolutie >= dropdownRezolutie.options.Count) indexRezolutie = 0;
            dropdownRezolutie.SetValueWithoutNotify(indexRezolutie);
        }

        AplicaSetariVideo(indexRezolutie, isFS);
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
        UpdateTextVolum(valoare);
    }

    public void AplicaSunet(bool activ)
    {
        if (activ)
        {
            sliderVolum.value = volumSalvat;
            AudioListener.volume = volumSalvat;
        }
        else
        {
            volumSalvat = sliderVolum.value > 0 ? sliderVolum.value : volumSalvat;
            sliderVolum.value = 0f;
            AudioListener.volume = 0f;
        }

        sliderVolum.interactable = activ;
        PlayerPrefs.SetInt("Sunet", activ ? 1 : 0);
        UpdateTextVolum(sliderVolum.value);
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

        List<string> optiuni = new List<string>
        {
            "1920 x 1080",
            "1600 x 900",
            "1366 x 768",
            "1280 x 720"
        };

        dropdownRezolutie.AddOptions(optiuni);
        dropdownRezolutie.RefreshShownValue();
    }

    public void SchimbaRezolutie(int index)
    {
        PlayerPrefs.SetInt("Rezolutie", index);
        bool isFS = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        
        AplicaSetariVideo(index, isFS);
    }

    public void SchimbaFullscreen(bool activ)
    {
        PlayerPrefs.SetInt("Fullscreen", activ ? 1 : 0);
        int index = PlayerPrefs.GetInt("Rezolutie", 0);

        AplicaSetariVideo(index, activ);
    }

    private void AplicaSetariVideo(int indexRezolutie, bool isFullscreen)
    {
        if (dropdownRezolutie == null || dropdownRezolutie.options.Count <= indexRezolutie) return;

        string[] dimensiuni = dropdownRezolutie.options[indexRezolutie].text.Split('x');
        
        if (dimensiuni.Length == 2 &&
            int.TryParse(dimensiuni[0].Trim(), out int w) &&
            int.TryParse(dimensiuni[1].Trim(), out int h))
        {
            FullScreenMode mode = isFullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
            
            Screen.SetResolution(w, h, mode);
        }
    }

    public void AplicaBright(float valoare)
    {
        float a = 1f - valoare;
        if (panelBright != null)
        {
            panelBright.color = new Color(0, 0, 0, a * 0.8f); 
        }
        PlayerPrefs.SetFloat("Luminozitate", valoare);
    }

    void UpdateTextVolum(float v)
    {
        if (textVolum != null) 
            textVolum.text = Mathf.RoundToInt(v * 100) + "%";
    }
}