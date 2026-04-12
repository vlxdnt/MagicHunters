using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        Unity.Netcode.NetworkObject netObj = other.GetComponent<Unity.Netcode.NetworkObject>();
        if (netObj == null || !netObj.IsOwner) return;

        triggered = true;
        gameObject.SetActive(false);

        ObjectivePressConnector connector = other.GetComponent<ObjectivePressConnector>();
        if (connector != null) connector.AvaseazaObiectivServerRpc();
    }
}