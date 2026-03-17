using Unity.Netcode;
using UnityEngine;

// Acest script se pune pe UIManager in MainMenu
// Se ocupa de spawn-ul jucatorilor in GameScene in functie de selectia din lobby
public class GameSpawner : MonoBehaviour
{
    [Header("Prefaburi")]
    public GameObject prefabWitch; // Prefabul pentru Vrajitoare
    public GameObject prefabCat;   // Prefabul pentru Pisica

    [Header("Puncte de Spawn (optional)")]
    public Transform spawnHost;   // Pozitia de spawn pentru Host
    public Transform spawnClient; // Pozitia de spawn pentru Client

    void OnEnable()
    {
        // Asteptam ca NetworkManager sa fie gata inainte sa ne abonam la eveniment
        StartCoroutine(AsteaptaNetworkManager());
    }

    System.Collections.IEnumerator AsteaptaNetworkManager()
    {
        // Asteptam sa existe NetworkManager
        while (NetworkManager.Singleton == null)
            yield return null;

        // Asteptam sa existe SceneManager
        while (NetworkManager.Singleton.SceneManager == null)
            yield return null;

        // Ne abonam la evenimentul de incarcare scena
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
    }

    // Se apeleaza automat cand o scena se incarca complet
    void OnSceneLoaded(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        // Ne intereseaza doar GameScene
        if (sceneName != "GameScene") return;
        // Doar serverul face spawn
        if (!NetworkManager.Singleton.IsServer) return;

        // Dezinregistram evenimentul ca sa nu se apeleze de mai multe ori
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;

        // Cautam selectiile din lobby
        int hostChoice = LobbySelection.finalHostSelection;
        int clientChoice = LobbySelection.finalClientSelection;

        Debug.Log("Host selection: " + hostChoice);
        Debug.Log("Client selection: " + clientChoice);

        // Verificam daca selectiile sunt valide
        if (hostChoice == 0 || clientChoice == 0)
        {
            Debug.LogWarning("GameSpawner: Unul dintre jucatori nu are selectia salvata corect!");
        }

        // Spawnam jucatorii cu prefaburile corecte
        SpawneazaJucator(0, hostChoice, spawnHost);
        SpawneazaJucator(1, clientChoice, spawnClient);
    }

    // Instantiaza si spawneaza prefabul corect pentru un jucator
    void SpawneazaJucator(ulong clientId, int selectie, Transform punct)
    {
        // selectie 1 = Witch, selectie 2 = Cat
        GameObject prefab = (selectie == 1) ? prefabWitch : prefabCat;

        // Folosim punctul de spawn daca exista, altfel pozitii default
        Vector3 pozitie = punct != null ? punct.position : (clientId == 0 ? new Vector3(-2, 0, 0) : new Vector3(2, 0, 0));

        GameObject obj = Instantiate(prefab, pozitie, Quaternion.identity);
        // Assignam obiectul ca player object pentru clientul respectiv
        obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    void OnDestroy()
    {
        // Dezabonam evenimentul cand obiectul e distrus ca sa evitam memory leaks
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
    }
}