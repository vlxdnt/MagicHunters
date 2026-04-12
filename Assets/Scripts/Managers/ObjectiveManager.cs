using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ObjectiveManager : NetworkBehaviour
{

    [Header("Chei obiective (din JSON)")]
    public string[] cheiObiective = {
    "obiectiv_1",
    "obiectiv_2",
    "obiectiv_3",
    "obiectiv_4",
    "obiectiv_5",
    "obiectiv_6"
    };
    public static ObjectiveManager Instance;

    private NetworkVariable<int> indexObiectiv = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        indexObiectiv.OnValueChanged += OnObiectivSchimbat;
        ActualizeazaUILocal(GetObiectivCurent()); // cheile
    }


    void OnObiectivSchimbat(int vechi, int nou)
    {
        ActualizeazaUILocal(GetObiectivCurent());
    }

    void ActualizeazaUILocal(string text)
    {
        // Aceasta ruleaza pe fiecare client care primeste schimbarea
        PlayerUI ui = FindFirstObjectByType<PlayerUI>(FindObjectsInactive.Include);
        if (ui != null)
            ui.SetObiectiv(text);
        else
            Debug.Log("PlayerUI nu a fost gasit! E activ?");
    }

    // apelare
    public void AvaseazaObiectiv()
    {
        if (!IsServer) return;
        Debug.Log("AvaseazaObiectiv apelat! Index curent: " + indexObiectiv.Value);
        int urmator = indexObiectiv.Value + 1;
        if (urmator < cheiObiective.Length)
        {
            indexObiectiv.Value = urmator;
            Debug.Log("Index nou: " + indexObiectiv.Value);
        }
    }

    public string GetObiectivCurent()
    {
        if (indexObiectiv.Value < cheiObiective.Length)
        {
            string cheie = cheiObiective[indexObiectiv.Value];
            // traducere
            if (Language.Instance != null)
                return Language.Instance.Get(cheie);
            return cheie;
        }
        return "";
    }
}