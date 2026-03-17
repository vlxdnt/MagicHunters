using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDisconnectHandler : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnect;
    }

    void OnPlayerDisconnect(ulong clientId)
    {
        if (NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }   

        SceneManager.LoadScene("MainMenu");

        if (MeniuManager.Instance != null)
        {
            MeniuManager.Instance.AfiseazaEroare("Un jucator s-a deconectat! S-a revenit la meniul principal.");
        }
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerDisconnect;
        }
    }
}
