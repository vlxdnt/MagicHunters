using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ObjectiveManager : NetworkBehaviour
{
    public static ObjectiveManager Instance;

    private NetworkVariable<int> indexObiectiv = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [Header("Obiective")]
    public string[] listaObiective = {
        "Explorati castelul!",
        "Gasiti cheia secreta!",
        "Deschideti usa mare!"
    };

    void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        indexObiectiv.OnValueChanged += OnObiectivSchimbat;
        // obiectiv initial
        ActualizeazaUILocal(listaObiective[0]);
    }

    void OnObiectivSchimbat(int vechi, int nou)
    {
        if (nou < listaObiective.Length)
            ActualizeazaUILocal(listaObiective[nou]);
    }

    void ActualizeazaUILocal(string text)
    {
        PlayerUI ui = FindFirstObjectByType<PlayerUI>();
        if (ui != null)
            ui.SetObiectiv(text);
    }

    // apelare
    public void AvaseazaObiectiv()
    {
        if (!IsServer) return;
        int urmator = indexObiectiv.Value + 1;
        if (urmator < listaObiective.Length)
            indexObiectiv.Value = urmator;
    }

    public string GetObiectivCurent()
    {
        if (indexObiectiv.Value < listaObiective.Length)
            return listaObiective[indexObiectiv.Value];
        return "";
    }
}