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
            // Trimitem mesajul la toți jucătorii înainte să îi deconectăm
            PregatesteVictorieClientRpc();
        }
    }

    [ClientRpc]
    private void PregatesteVictorieClientRpc()
    {
        // "Păcălim" variabila de eroare cu mesajul de victorie
        MeniuManager.eroareIntreScene = "Felicitari, ai invins vrajitoarea cea rea!";

        // Închidem conexiunea imediat
        if (NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }
        
        // NetworkManager.Singleton.Shutdown() va declanșa automat 
        // funcția OnPlayerDisconnect din scriptul tău existent!
    }
}