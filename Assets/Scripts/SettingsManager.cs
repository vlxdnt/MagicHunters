using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public GameObject panelSettings;
    public GameObject panelMeniu;

    public Toggle toggleSunet;
    public Toggle toggleFullscreen;

    public TextMeshProUGUI textVolum;
    public Slider sliderVolum;
    public TMP_Dropdown dropdownRezolutie;

    private float volumSalvat = 1f;

    private void Start()
    {
        PopuleazaRezolutii();
        volumSalvat = PlayerPrefs.GetFloat("Volum", 1f);

        toggleFullscreen.SetIsOnWithoutNotify(PlayerPrefs.GetInt("Fullscreen", 1) == 1);
        Screen.fullScreen = toggleFullscreen.isOn;

        // Setam valorile fara sa declansam eventurile
        sliderVolum.SetValueWithoutNotify(volumSalvat);
        toggleSunet.SetIsOnWithoutNotify(PlayerPrefs.GetInt("Sunet", 1) == 1);

        // Aplicam manual o singura data
        bool sunetActiv = toggleSunet.isOn;
        sliderVolum.interactable = sunetActiv;
        AudioListener.volume = sunetActiv ? volumSalvat : 0f;
        if (textVolum != null)
            textVolum.text = Mathf.RoundToInt(sunetActiv ? volumSalvat * 100 : 0) + "%";
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
            textVolum.text = Mathf.RoundToInt(valoare * 100) + "%";
    }

    public void AplicaSunet(bool activ)
    {
        if (activ)
        {
            // Restauram volumul salvat
            sliderVolum.value = volumSalvat;
            AudioListener.volume = volumSalvat;
        }
        else
        {
            // Salvam volumul curent si dam la 0
            volumSalvat = sliderVolum.value > 0 ? sliderVolum.value : volumSalvat;
            sliderVolum.value = 0f;
            AudioListener.volume = 0f;
        }

        sliderVolum.interactable = activ;
        PlayerPrefs.SetInt("Sunet", activ ? 1 : 0);

        if (textVolum != null)
            textVolum.text = Mathf.RoundToInt(sliderVolum.value * 100) + "%";
    }

    public void SchimbaRezolutie(int index)
    {
        Resolution r = Screen.resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Rezolutie", index);
    }

    void PopuleazaRezolutii()
    {
        dropdownRezolutie.ClearOptions();
        var optiuni = new System.Collections.Generic.List<string>();
        foreach (var r in Screen.resolutions)
            optiuni.Add(r.width + " x " + r.height);
        dropdownRezolutie.AddOptions(optiuni);

        // Setam rezolutia salvata anterior
        dropdownRezolutie.value = PlayerPrefs.GetInt("Rezolutie", 0);
        dropdownRezolutie.RefreshShownValue();
    }

    public void SchimbaFullscreen(bool activ)
    {
        Screen.fullScreen = activ;
        PlayerPrefs.SetInt("Fullscreen", activ ? 1 : 0);
    }
}