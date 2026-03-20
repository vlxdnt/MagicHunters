using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// lobby menu
public class LobbySelection : NetworkBehaviour
{
    // selectia jucatorilor, nrjucatori
    public NetworkVariable<int> hostSelection = new NetworkVariable<int>(0);
    public NetworkVariable<int> clientSelection = new NetworkVariable<int>(0);
    public NetworkVariable<int> nrJucatori = new NetworkVariable<int>(0);
    public static int finalHostSelection = 0;
    public static int finalClientSelection = 0;
    private MeniuManager meniu;

    //
    [Header("Butoane selectie personaj")]
    public Button butonWitch;
    public Button butonCat;

    [Header("Buton iesire lobby")]
    public Button butonQuit;

    [Header("Text")]
    public TextMeshProUGUI textJucatori;
    public TextMeshProUGUI textSelectieHost;
    public TextMeshProUGUI textSelectieClient;

    // spawn in retea
    public override void OnNetworkSpawn()
    {
        meniu = FindFirstObjectByType<MeniuManager>();
        if (IsServer)
        {
            // nr initial de jucatori
            nrJucatori.Value = NetworkManager.Singleton.ConnectedClients.Count;
            // connect/disconnect
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    // la un nou connect
    void OnClientConnected(ulong clientId)
    {
        nrJucatori.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    // la disconn
    void OnClientDisconnected(ulong clientId)
    {
        nrJucatori.Value = NetworkManager.Singleton.ConnectedClients.Count;
        // reset la selectie in caz de disconn
        if (clientId != 0)
            clientSelection.Value = 0;
    }

    public void TrimiteeFadeInTuturor()
    {
        FadeInRpc();
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    void FadeInRpc()
    {
        if (SceneFade.Instance != null)
            SceneFade.Instance.StartCoroutine(SceneFade.Instance.FadeIn(0.5f));
    }

    // apelate in lobby
    public void AlegeWitch() { TrimiteAlegereaRpc(1, NetworkManager.Singleton.LocalClientId); }
    public void AlegeCat() { TrimiteAlegereaRpc(2, NetworkManager.Singleton.LocalClientId); }

    // salvare selectie pe server
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void TrimiteAlegereaRpc(int choice, ulong clientId)
    {
        Debug.Log("RPC primit! ClientId: " + clientId + " Choice: " + choice);
        if (clientId == 0)
            hostSelection.Value = choice;
        else
            clientSelection.Value = choice;
    }

    // quit, in functie de rol
    public void QuitLobby()
    {
        // nu e conectat
        if (!IsSpawned) return;
        if (IsHost)
            QuitHostRpc();
        else
            QuitClientRpc();
    }

    // hostul iese, reset
    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Everyone)]
    void QuitHostRpc()
    {
        NetworkManager.Singleton.Shutdown();
        MeniuManager meniu = FindFirstObjectByType<MeniuManager>();
        if (meniu != null) meniu.ResetLobby();
    }

    // clientul iese, reset la selectie si disconn
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void QuitClientRpc()
    {
        clientSelection.Value = 0;
        nrJucatori.Value = NetworkManager.Singleton.ConnectedClients.Count - 1;
        NetworkManager.Singleton.DisconnectClient(1);
        // schimb de meniu
        ReturnClientToMenuRpc();
    }

    // schimb de meniu, doar pt client
    [Rpc(SendTo.NotServer, InvokePermission = RpcInvokePermission.Server)]
    void ReturnClientToMenuRpc()
    {
        NetworkManager.Singleton.Shutdown();
        MeniuManager meniu = FindFirstObjectByType<MeniuManager>();
        if (meniu != null) meniu.ResetLobby();
    }

    void Update()
    {
        // 
        if (!IsSpawned) return;

        // nr de jucatori
        if (textJucatori != null)
            textJucatori.text = "Jucatori conectati: " + nrJucatori.Value + "/2";

        // deblocare la nr bun de jucatori
        if (NetworkManager.Singleton.IsHost)
        {
            if (meniu != null && meniu.butonStart != null)
                meniu.butonStart.interactable = (nrJucatori.Value == 2 && hostSelection.Value != 0 && clientSelection.Value != 0 && hostSelection.Value != clientSelection.Value);
        }

        if (butonWitch == null || butonCat == null) return;

        if (hostSelection.Value != 0 && textSelectieHost != null)
        {
            textSelectieHost.text = "Host: " + (hostSelection.Value == 1 ? "Witch" : "Cat");
        }
        if (clientSelection.Value != 0 && textSelectieClient != null)
        {
            textSelectieClient.text = "Client: " + (clientSelection.Value == 1 ? "Witch" : "Cat");
        }
    }

    public override void OnDestroy()
    {
        // distrugere obiect la callback
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}