using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbySelection : NetworkBehaviour
{
    // 0 = Nimic, 1 = Witch, 2 = Cat
    public NetworkVariable<int> hostSelection = new NetworkVariable<int>(0);
    public NetworkVariable<int> clientSelection = new NetworkVariable<int>(0);
    public NetworkVariable<int> nrJucatori = new NetworkVariable<int>(0);

    public Button butonWitch;
    public Button butonCat;
    public TextMeshProUGUI textJucatori;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            nrJucatori.Value = NetworkManager.Singleton.ConnectedClients.Count;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    void OnClientConnected(ulong clientId)
    {
        nrJucatori.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    public void AlegeWitch()
    {
        if (!IsSpawned) return;
        TrimiteAlegereaRpc(1, NetworkManager.Singleton.LocalClientId);
    }

    public void AlegeCat()
    {
        if (!IsSpawned) return;
        TrimiteAlegereaRpc(2, NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void TrimiteAlegereaRpc(int choice, ulong clientId)
    {
        if (clientId == 0)
            hostSelection.Value = choice;
        else
            clientSelection.Value = choice;
    }

    void Update()
    {
        if (textJucatori != null)
            textJucatori.text = "Jucatori conectati: " + nrJucatori.Value + "/2";

        if (NetworkManager.Singleton.IsHost)
        {
            MeniuManager meniu = FindFirstObjectByType<MeniuManager>();
            if (meniu != null && meniu.butonStart != null)
                meniu.butonStart.interactable = (nrJucatori.Value == 2);
        }

        if (butonWitch == null || butonCat == null) return;

        if (IsHost)
        {
            butonWitch.interactable = (clientSelection.Value != 1);
            butonCat.interactable = (clientSelection.Value != 2);
        }
        else
        {
            butonWitch.interactable = (hostSelection.Value != 1);
            butonCat.interactable = (hostSelection.Value != 2);
        }
    }
}