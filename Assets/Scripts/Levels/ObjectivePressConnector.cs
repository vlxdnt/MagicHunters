using UnityEngine;
using Unity.Netcode;

public class ObjectivePressConnector : NetworkBehaviour
{
    [ServerRpc]
    public void AvaseazaObiectivServerRpc(ulong triggerNetworkId)
    {
        if (ObjectiveManager.Instance != null)
            ObjectiveManager.Instance.AvaseazaObiectiv();

        // dezactiv triggeru pt toti
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(triggerNetworkId, out NetworkObject triggerObj))
        {
            DezactiveazaTriggerClientRpc(triggerNetworkId);
        }
    }

    [ClientRpc]
    void DezactiveazaTriggerClientRpc(ulong triggerNetworkId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(triggerNetworkId, out NetworkObject triggerObj))
        {
            triggerObj.gameObject.SetActive(false);
        }
    }
}