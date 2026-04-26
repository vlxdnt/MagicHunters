using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class LevelTransition : NetworkBehaviour
{
    [Header("Setari")]
    public Transform spawnUrmatorNivel;
    public float fadeDuration = 0.5f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sunetTranzitie;

    private HashSet<ulong> jucatoriInZona = new HashSet<ulong>();
    private bool triggered = false;

    // DOAR SERVERUL proceseaza intrarile, fara RPC-uri care dau crash!
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
        yield return new WaitForSeconds(fadeDuration + 0.1f);

        SchimbaNivelClientRpc(spawnUrmatorNivel.position);
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
    void SchimbaNivelClientRpc(Vector3 pozitieNoua)
    {
        PlayerInput[] players = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (player.IsOwner)
                player.transform.position = pozitieNoua;
        }

        // fade in
        if (SceneFade.Instance != null)
            StartCoroutine(SceneFade.Instance.FadeIn(fadeDuration));
    }
}