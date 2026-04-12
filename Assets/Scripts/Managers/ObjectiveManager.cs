using UnityEngine;
using Unity.Netcode;

public class ObjectiveManager : NetworkBehaviour
{
    public static ObjectiveManager Instance;

    [Header("Cheite din json")]
    public string[] cheiObiective = {
        "obiectiv_0", "obiectiv_1", "obiectiv_2",
        "obiectiv_3", "obiectiv_4", "obiectiv_5", "obiectiv_6"
    };

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
    }

    void OnObiectivSchimbat(int vechi, int nou)
    {
        ActualizeazaUI();
    }

    void ActualizeazaUI()
    {
        string text = GetObiectivCurent();
        PlayerUI ui = FindFirstObjectByType<PlayerUI>(FindObjectsInactive.Include);
        if (ui != null) ui.SetObiectiv(text);
    }

    public void AvaseazaObiectiv()
    {
        if (!IsServer) return;
        int urmator = indexObiectiv.Value + 1;
        if (urmator < cheiObiective.Length)
            indexObiectiv.Value = urmator;
    }

    public string GetObiectivCurent()
    {
        if (indexObiectiv.Value < cheiObiective.Length)
        {
            string cheie = cheiObiective[indexObiectiv.Value];
            if (Language.Instance != null)
                return Language.Instance.Get(cheie);
            return cheie;
        }
        return "";
    }

    public int GetIndexCurent() => indexObiectiv.Value;

    public override void OnNetworkDespawn()
    {
        indexObiectiv.OnValueChanged -= OnObiectivSchimbat;
    }
}