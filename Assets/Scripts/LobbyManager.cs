using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Gestioneaza lobby-ul: selectia personajelor, numarul de jucatori conectati si iesirea din lobby
public class LobbySelection : NetworkBehaviour
{
    // Selectiile jucatorilor sincronizate prin retea: 0 = Nimic, 1 = Witch, 2 = Cat
    public NetworkVariable<int> hostSelection = new NetworkVariable<int>(0);
    public NetworkVariable<int> clientSelection = new NetworkVariable<int>(0);
    public NetworkVariable<int> nrJucatori = new NetworkVariable<int>(0);

    [Header("Butoane selectie personaj")]
    public Button butonWitch;
    public Button butonCat;

    [Header("Buton iesire lobby")]
    public Button butonQuit;

    [Header("Text nr jucatori")]
    public TextMeshProUGUI textJucatori;

    // Se apeleaza cand obiectul e spawnat in retea
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Setam numarul initial de jucatori
            nrJucatori.Value = NetworkManager.Singleton.ConnectedClients.Count;
            // Ascultam pentru conectari si deconectari
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    // Se apeleaza cand un client nou se conecteaza
    void OnClientConnected(ulong clientId)
    {
        nrJucatori.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    // Se apeleaza cand un client se deconecteaza
    void OnClientDisconnected(ulong clientId)
    {
        nrJucatori.Value = NetworkManager.Singleton.ConnectedClients.Count - 1;
        // Resetam selectia clientului daca el a iesit
        if (clientId != 0)
            clientSelection.Value = 0;
    }

    // Functii apelate de butoanele din lobby
    public void AlegeWitch() { TrimiteAlegereaRpc(1, NetworkManager.Singleton.LocalClientId); }
    public void AlegeCat() { TrimiteAlegereaRpc(2, NetworkManager.Singleton.LocalClientId); }

    // RPC trimis catre server pentru a salva selectia jucatorului
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void TrimiteAlegereaRpc(int choice, ulong clientId)
    {
        Debug.Log("RPC primit! ClientId: " + clientId + " Choice: " + choice);
        if (clientId == 0)
            hostSelection.Value = choice;
        else
            clientSelection.Value = choice;
    }

    // Apelata de butonul Quit - determina ce actiune sa faca in functie de rol
    public void QuitLobby()
    {
        // Nu facem nimic daca nu suntem conectati
        if (!IsSpawned) return;
        if (IsHost)
            QuitHostRpc();
        else
            QuitClientRpc();
    }

    // Hostul iese - trimitem toti jucatorii la meniu si oprim reteaua
    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Everyone)]
    void QuitHostRpc()
    {
        NetworkManager.Singleton.Shutdown();
        MeniuManager meniu = FindFirstObjectByType<MeniuManager>();
        if (meniu != null) meniu.ResetLobby();
    }

    // Clientul iese - resetam selectia lui si il deconectam
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void QuitClientRpc()
    {
        clientSelection.Value = 0;
        nrJucatori.Value = NetworkManager.Singleton.ConnectedClients.Count - 1;
        NetworkManager.Singleton.DisconnectClient(1);
        // Trimitem clientul inapoi la meniu
        ReturnClientToMenuRpc();
    }

    // Se trimite doar catre client (nu server) ca sa se intoarca la meniu
    [Rpc(SendTo.NotServer, InvokePermission = RpcInvokePermission.Server)]
    void ReturnClientToMenuRpc()
    {
        NetworkManager.Singleton.Shutdown();
        MeniuManager meniu = FindFirstObjectByType<MeniuManager>();
        if (meniu != null) meniu.ResetLobby();
    }

    void Update()
    {
        // Nu rulam Update daca nu suntem conectati
        if (!IsSpawned) return;

        // Actualizam textul cu numarul de jucatori
        if (textJucatori != null)
            textJucatori.text = "Jucatori conectati: " + nrJucatori.Value + "/2";

        // Deblocam butonul Start doar daca sunt 2 jucatori si ambii au ales
        if (NetworkManager.Singleton.IsHost)
        {
            MeniuManager meniu = FindFirstObjectByType<MeniuManager>();
            if (meniu != null && meniu.butonStart != null)
                meniu.butonStart.interactable = (nrJucatori.Value == 2 && hostSelection.Value != 0 && clientSelection.Value != 0);
        }

        if (butonWitch == null || butonCat == null) return;

        if (IsHost)
        {
            // Dezactivam butonul deja ales de client
            butonWitch.interactable = (clientSelection.Value != 1);
            butonCat.interactable = (clientSelection.Value != 2);
            // Coloram verde selectia hostului
            butonWitch.GetComponent<Image>().color = (hostSelection.Value == 1) ? Color.green : Color.white;
            butonCat.GetComponent<Image>().color = (hostSelection.Value == 2) ? Color.green : Color.white;
        }
        else
        {
            // Dezactivam butonul deja ales de host
            butonWitch.interactable = (hostSelection.Value != 1);
            butonCat.interactable = (hostSelection.Value != 2);
            // Coloram albastru selectia clientului
            butonWitch.GetComponent<Image>().color = (clientSelection.Value == 1) ? Color.blue : Color.white;
            butonCat.GetComponent<Image>().color = (clientSelection.Value == 2) ? Color.blue : Color.white;
        }
    }

    public override void OnDestroy()
    {
        // Dezabonam callback-urile cand obiectul e distrus
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}