using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDisconnectHandler : NetworkBehaviour
{
    // Variabila globala pe care o va modifica usa de final
    public static bool victorieObtinuta = false;

    public override void OnNetworkSpawn()
    {
        victorieObtinuta = false; // Resetam la inceputul meciului
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnect;
    }

    void OnPlayerDisconnect(ulong clientId)
{
    if (NetworkManager.Singleton.IsListening)
    {
        NetworkManager.Singleton.Shutdown();
    }   

    // Verificăm: dacă textul e gol, înseamnă că e eroare neprevăzută.
    // Dacă are deja ceva în el (scris de ușa de victorie), îl lăsăm așa!
    if (string.IsNullOrEmpty(MeniuManager.eroareIntreScene))
    {
        MeniuManager.eroareIntreScene = "Un jucator s-a deconectat. S-a revenit la meniu.";
    }

    SceneManager.LoadScene("MainMenu");
}

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerDisconnect;
        }
    }
}