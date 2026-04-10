using UnityEngine;
using Unity.Netcode;

public class PlayerUIConnector : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        PlayerUI ui = FindFirstObjectByType<PlayerUI>();
        if (ui != null)
        {
            bool isWitch = GetComponent<WitchAnimator>() != null;
            ui.SetPersonaj(isWitch, NetworkManager.Singleton.LocalClientId);
        }
    }
}