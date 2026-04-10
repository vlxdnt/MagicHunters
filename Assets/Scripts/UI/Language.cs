using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;

// manipuleaza json
public class Language : MonoBehaviour
{
    public static Language Instance;

    // event pt schimbarea tutoror txtmsh
    public static event System.Action OnLimbaSchimbata;

    public enum Limba { Romana, Engleza }
    public Limba limbaActuala = Limba.Romana;

    // dictionaru efectiv din json
    private Dictionary<string, Dictionary<string, string>> texte;

    public TMP_Dropdown dropdownLimba;

    void Start()
    {
        //populare cu optiuni
        dropdownLimba.ClearOptions();
        dropdownLimba.AddOptions(new System.Collections.Generic.List<string> { "Romana", "English" });

        int limbaSalvata = PlayerPrefs.GetInt("Limba", 0);
        dropdownLimba.SetValueWithoutNotify(limbaSalvata);
        dropdownLimba.RefreshShownValue();

        // aplicam limba salvata
        SchimbaLimba(limbaSalvata);
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            IncarcaJSON(); //incarcare
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        DontDestroyOnLoad(gameObject);

        limbaActuala = (Limba)PlayerPrefs.GetInt("Limba", 0);
        IncarcaJSON();
    }

    void IncarcaJSON()
    {
        // incarc json
        TextAsset json = Resources.Load<TextAsset>("limba");
        if (json == null)
        {
            Debug.LogError("Nu exista fisierul");
            return;
        }
        texte = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json.text);
        Debug.Log("Json cu " + texte.Count + " chei.");
    }

    // returneaza limba in functie de cheie si cod
    public string Get(string cheie)
    {
        if (texte == null) return cheie;
        string codLimba = limbaActuala == Limba.Romana ? "ro" : "en";
        if (texte.ContainsKey(cheie) && texte[cheie].ContainsKey(codLimba))
            return texte[cheie][codLimba];

        //
        return cheie;
    }

    // dropdown
    public void SchimbaLimba(int index)
    {
        Debug.Log("Schimb limba la: " + index);
        limbaActuala = (Limba)index;
        PlayerPrefs.SetInt("Limba", index);
        //actualizare
        OnLimbaSchimbata?.Invoke();
    }
}

