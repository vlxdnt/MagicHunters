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

    [Header("Scena de intro")]
    public IntroTimelineManager introManager;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // asteptam ecranu de loading
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnToataLumeaIncarcata;
        }
    }

    void OnToataLumeaIncarcata(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName != "GameScene") return;

        // var pt obiecte
        GameObject hostObj = null;
        GameObject clientObj = null;

        // spawn host
        hostObj = SpawneazaJucator(NetworkManager.ServerClientId, LobbySelection.finalHostSelection, spawnHost);

        // spawn client
        foreach (ulong clientId in clientsCompleted)
        {
            if (clientId != NetworkManager.ServerClientId)
            {
                clientObj = SpawneazaJucator(clientId, LobbySelection.finalClientSelection, spawnClient);
            }
        }

        // pt timeline, identificam jucatorii
        GameObject witchObj = null;
        GameObject catObj = null;

        // verificare alegere host
        if (LobbySelection.finalHostSelection == 1) witchObj = hostObj;
        else catObj = hostObj;

        // lfl pt client
        if (LobbySelection.finalClientSelection == 1) witchObj = clientObj;
        else catObj = clientObj;

        // pornire timeline pe server
        if (IsServer && introManager != null && witchObj != null && catObj != null)
        {
            introManager.LeagaPersonaje(witchObj, catObj);
        }
    }

    // returnam obiectele
    GameObject SpawneazaJucator(ulong clientId, int selectie, Transform punct)
    {
        GameObject prefab = (selectie == 1) ? prefabWitch : prefabCat;
        if (prefab == null) return null;

        Vector3 pozitie = punct != null ? punct.position : (clientId == NetworkManager.ServerClientId ? new Vector3(-2, 0, 0) : new Vector3(2, 0, 0));

        GameObject obj = Instantiate(prefab, pozitie, Quaternion.identity);
        obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

        return obj;
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnToataLumeaIncarcata;
    }
}