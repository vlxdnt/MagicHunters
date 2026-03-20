using Unity.Netcode;
using UnityEngine;

// se ocupa de spawn ul ambilor jucatori 
public class GameSpawner : MonoBehaviour
{
    //pentru caractere
    [Header("Prefaburi")]
    public GameObject prefabWitch;
    public GameObject prefabCat;

    //spawnpoints - posibil de eliminat pe viitor in functie de cutscene-ul de inceput
    [Header("Puncte de Spawn (optional)")]
    public Transform spawnHost;   
    public Transform spawnClient; 

    void OnEnable()
    {
        //wait
        StartCoroutine(AsteaptaNetworkManager());
    }

    System.Collections.IEnumerator AsteaptaNetworkManager()
    {
        while (NetworkManager.Singleton == null)
            yield return null;

        while (NetworkManager.Singleton.SceneManager == null)
            yield return null;

        // incarcare scena
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
    }

    void OnSceneLoaded(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        if (sceneName != "GameScene") return;
        //
        if (!NetworkManager.Singleton.IsServer) return;

        // sa nu faca reapel
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;

        // selectia jucatorilor
        int hostChoice = LobbySelection.finalHostSelection;
        int clientChoice = LobbySelection.finalClientSelection;

        Debug.Log("Host selection: " + hostChoice);
        Debug.Log("Client selection: " + clientChoice);

        // verificare
        if (hostChoice == 0 || clientChoice == 0)
        {
            Debug.LogWarning("GameSpawner: Unul dintre jucatori nu are selectia salvata corect!");
        }

        // spawn ul jucatorilor
        SpawneazaJucator(0, hostChoice, spawnHost);
        SpawneazaJucator(1, clientChoice, spawnClient);

        LobbySelection lobby = FindFirstObjectByType<LobbySelection>();
        if (lobby != null)
            lobby.TrimiteeFadeInTuturor();
        else
            Debug.LogError("Nu gasesc LobbySelection");

        lobby.TrimiteeFadeInTuturor();
    }

    // pentru fiecare jucator
    void SpawneazaJucator(ulong clientId, int selectie, Transform punct)
    {
        // selectie
        GameObject prefab = (selectie == 1) ? prefabWitch : prefabCat;

        // daca exista, daca nu, default pos
        Vector3 pozitie = punct != null ? punct.position : (clientId == 0 ? new Vector3(-2, 0, 0) : new Vector3(2, 0, 0));

        GameObject obj = Instantiate(prefab, pozitie, Quaternion.identity);
        // player object
        obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        StartCoroutine(SceneFade.Instance.FadeIn(0.5f));
    }

    void OnDestroy()
    {
        // memory leak
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
    }
}