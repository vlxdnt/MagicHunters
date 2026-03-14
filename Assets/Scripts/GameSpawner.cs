using Unity.Netcode;
using UnityEngine;

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
        if (!IsServer) return;

        LobbySelection lobby = FindFirstObjectByType<LobbySelection>();
        if (lobby == null)
        {
            Debug.LogError("GameSpawner: Nu gasesc LobbySelection!");
            return;
        }

        SpawneazaJucator(0, lobby.hostSelection.Value, spawnHost);
        SpawneazaJucator(1, lobby.clientSelection.Value, spawnClient);
    }

    void SpawneazaJucator(ulong clientId, int selectie, Transform punct)
    {
        GameObject prefab = (selectie == 2) ? prefabCat : prefabWitch;
        Vector3 pozitie = punct != null ? punct.position : (clientId == 0 ? new Vector3(-2, 0, 0) : new Vector3(2, 0, 0));

        GameObject obj = Instantiate(prefab, pozitie, Quaternion.identity);
        obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
}