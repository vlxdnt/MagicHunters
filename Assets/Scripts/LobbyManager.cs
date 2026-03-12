using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbySelection : NetworkBehaviour
{
    // 0 = Nimic, 1 = Witch, 2 = Cat
    public NetworkVariable<int> hostSelection = new NetworkVariable<int>(0);
    public NetworkVariable<int> clientSelection = new NetworkVariable<int>(0);

    public Button butonWitch;
    public Button butonCat;

    public void AlegeWitch() { TrimiteAlegereaServerRpc(1); }
    public void AlegeCat() { TrimiteAlegereaServerRpc(2); }

    [ServerRpc(RequireOwnership = false)]
    void TrimiteAlegereaServerRpc(int choice, ServerRpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId == 0) // E Hostul
            hostSelection.Value = choice;
        else // E Clientul
            clientSelection.Value = choice;
    }

    void Update()
    {
        // Dezactivăm butoanele dacă celălalt a ales deja
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
        //test
    }
}
