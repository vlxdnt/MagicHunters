using UnityEngine;
using Unity.Netcode;

public class ObjectiveTrigger : NetworkBehaviour
{
    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj == null || !netObj.IsOwner) return;

        triggered = true;

        ObjectivePressConnector connector = other.GetComponent<ObjectivePressConnector>();
        if (connector != null) connector.AvaseazaObiectivServerRpc(NetworkObjectId);
    }
}