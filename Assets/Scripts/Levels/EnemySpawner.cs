using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public bool spawnLaInceput = false; // Daca e bifat, apare imediat. Daca nu, asteapta comanda.

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        if (spawnLaInceput)
        {
            SpawnEnemy();
        }
    }

    // Aceasta functie va fi chemata de usa de la nivel
    public void SpawnEnemy()
    {
        if (!IsServer || enemyPrefab == null) return;

        GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
        enemy.GetComponent<NetworkObject>().Spawn();
        
        // Dupa ce a spawnat inamicul, spawner-ul se poate distruge singur
        Destroy(gameObject); 
    }
}