using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class CastleEntrance : NetworkBehaviour
{
    [Header("Setari Teleportare")]
    public float fadeDuration = 0.5f;

    [Header("Nivele")]
    public GameObject nivelCurent;
    public GameObject nivelUrmator;
    public GameObject margins;

    [Header("Sistem Spawner Inamici")]
    public EnemySpawner[] spawneriDinNoulNivel; // Aici tragi spawner-ele din noul nivel

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sunetTranzitie;

    // Folosim un HashSet pentru a nu numara acelasi jucator de 2 ori pe server
    private HashSet<ulong> jucatoriInZona = new HashSet<ulong>();
    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer || triggered) return;
        if (!other.CompareTag("Player")) return;

        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj != null && netObj.IsPlayerObject)
        {
            jucatoriInZona.Add(netObj.OwnerClientId);
            VerificaTranzitie();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsServer || triggered) return;
        if (!other.CompareTag("Player")) return;

        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj != null && netObj.IsPlayerObject)
        {
            jucatoriInZona.Remove(netObj.OwnerClientId);
        }
    }

    void VerificaTranzitie()
    {
        // Cati jucatori sunt pe server acum?
        int jucatoriConectati = NetworkManager.Singleton.ConnectedClients.Count;
        
        // Daca toti jucatorii conectati sunt la usa (suporta 1 singur jucator sau 2)
        if (jucatoriInZona.Count >= jucatoriConectati && jucatoriConectati > 0)
        {
            triggered = true;
            StartCoroutine(TeleportareNivel());
        }
    }

    IEnumerator TeleportareNivel()
    {
        FadeOutClientRpc();
        
        // Asteptam sa se faca ecranul negru
        yield return new WaitForSeconds(fadeDuration + 0.1f);

        // ---> AICI ESTE PARTEA ADAUGATA CARE LIPSEA <---
        SchimbaNivelClientRpc();
    }

    [ClientRpc]
    void FadeOutClientRpc()
    {
        if (audioSource != null && sunetTranzitie != null)
            audioSource.PlayOneShot(sunetTranzitie);

        if (SceneFade.Instance != null)
            StartCoroutine(SceneFade.Instance.FadeOut(fadeDuration));
    }

    [ClientRpc]
    void SchimbaNivelClientRpc() // S-a scos (Vector3 pozitieNoua) pt ca nu e necesar aici
    {
        // 1. Schimbam vizibilitatea nivelelor
        if (nivelCurent != null) nivelCurent.SetActive(false);
        if (nivelUrmator != null) nivelUrmator.SetActive(true);
        if (margins != null) margins.SetActive(false);

        // 2. SERVERUL activeaza spawner-ele din camera noua
        if (IsServer)
        {
            foreach (EnemySpawner spawner in spawneriDinNoulNivel)
            {
                if (spawner != null)
                {
                    spawner.SpawnEnemy();
                }
            }
        }

        // 4. Fade In - revenim la lumina
        if (SceneFade.Instance != null)
            StartCoroutine(SceneFade.Instance.FadeIn(fadeDuration));
    }
}