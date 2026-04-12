using UnityEngine;
using Unity.Netcode;

public class ButtonPressConnector : NetworkBehaviour
{
    [ServerRpc]
    public void ApasaButonServerRpc(string numeButon)
    {
        ApasaButonClientRpc(numeButon);
    }

    [ClientRpc]
    void ApasaButonClientRpc(string numeButon)
    {
        if (IsOwner) return; //pt owner local

        GameObject buton = GameObject.Find(numeButon);
        if (buton != null)
        {
            ButtonDoor bd = buton.GetComponent<ButtonDoor>();
            if (bd != null) bd.ApasaButon();
        }
    }
}