using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class GameSpawner : NetworkBehaviour
{
    [Header("Prefaburi")]
    public GameObject prefabWitch;
    public GameObject prefabCat;

    [Header("Puncte de Spawn (optional)")]
    public Transform spawnHost;   
    public Transform spawnClient; 

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Asteptam ca TOTI jucatorii sa termine ecranul de loading
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnToataLumeaIncarcata;
        }
    }

    void OnToataLumeaIncarcata(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName != "GameScene") return;

        // 1. Spawnam Host-ul abia acum
        SpawneazaJucator(NetworkManager.ServerClientId, LobbySelection.finalHostSelection, spawnHost);

        // 2. Spawnam si Clientii
        foreach (ulong clientId in clientsCompleted)
        {
            if (clientId != NetworkManager.ServerClientId)
            {
                SpawneazaJucator(clientId, LobbySelection.finalClientSelection, spawnClient);
            }
        }
    }

    void SpawneazaJucator(ulong clientId, int selectie, Transform punct)
    {
        GameObject prefab = (selectie == 1) ? prefabWitch : prefabCat;
        if (prefab == null) return;

        Vector3 pozitie = punct != null ? punct.position : (clientId == NetworkManager.ServerClientId ? new Vector3(-2, 0, 0) : new Vector3(2, 0, 0));

        GameObject obj = Instantiate(prefab, pozitie, Quaternion.identity);
        obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }

    void Start()
    {
        if (SceneFade.Instance != null) StartCoroutine(SceneFade.Instance.FadeIn(0.5f));
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnToataLumeaIncarcata;
    }
}