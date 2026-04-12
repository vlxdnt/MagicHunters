using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class VictoryDoor : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            PregatesteVictorieClientRpc();
        }
    }

    [ClientRpc]
    private void PregatesteVictorieClientRpc()
    {
        MeniuManager.eroareIntreScene = "Felicitari, ai invins vrajitoarea cea rea!";

        if (NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

    }
}