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
        PopuleazaRezolutii(); //setarea pt dropdown

        volumSalvat = PlayerPrefs.GetFloat("Volum", 1f); //ce are salvat deja dupa ce da enable inapoi

        toggleFullscreen.SetIsOnWithoutNotify(PlayerPrefs.GetInt("Fullscreen", 1) == 1);
        Screen.fullScreen = toggleFullscreen.isOn; //setarea pt ecran

        sliderVolum.SetValueWithoutNotify(volumSalvat);
        toggleSunet.SetIsOnWithoutNotify(PlayerPrefs.GetInt("Sunet", 1) == 1);

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

        // rezolutia anterioara
        dropdownRezolutie.value = PlayerPrefs.GetInt("Rezolutie", 0);
        dropdownRezolutie.RefreshShownValue();
    }

    public void SchimbaFullscreen(bool activ)
    {
        Screen.fullScreen = activ;
        PlayerPrefs.SetInt("Fullscreen", activ ? 1 : 0);
    }
}