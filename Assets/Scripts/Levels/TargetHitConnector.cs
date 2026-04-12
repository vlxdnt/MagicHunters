using UnityEngine;
using Unity.Netcode;

public class TargetHitConnector : NetworkBehaviour
{
    [ServerRpc]
    public void LovesteTargetServerRpc(string numeTarget)
    {
        LovesteTargetClientRpc(numeTarget);
    }

    [ClientRpc]
    void LovesteTargetClientRpc(string numeTarget)
    {
        if (IsOwner) return;

        GameObject target = GameObject.Find(numeTarget);
        if (target != null)
        {
            TargetDoor td = target.GetComponent<TargetDoor>();
            if (td != null) td.DeschideUsa();
        }
    }
}