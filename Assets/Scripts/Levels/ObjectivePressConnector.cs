using UnityEngine;
using Unity.Netcode;

public class ObjectivePressConnector : NetworkBehaviour
{
    [ServerRpc]
    public void AvaseazaObiectivServerRpc()
    {
        if (ObjectiveManager.Instance != null)
            ObjectiveManager.Instance.AvaseazaObiectiv();
    }
}