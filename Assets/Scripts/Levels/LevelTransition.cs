using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class LevelTransition : NetworkBehaviour
{
    [Header("Setari")]
    public Transform spawnUrmatorNivel;
    public float fadeDuration = 0.5f;

    [Header("Nivele")]
    public GameObject nivelCurent;
    public GameObject nivelUrmator;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sunetTranzitie;

    private int jucatoriInZona = 0;
    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        // Doar owner-ul notifica serverul
        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj == null || !netObj.IsOwner) return;

        IntraInZonaServerRpc();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj == null || !netObj.IsOwner) return;

        IeseZonaServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void IntraInZonaServerRpc()
    {
        if (triggered) return;
        jucatoriInZona++;
        Debug.Log("Jucatori in zona: " + jucatoriInZona);
        if (jucatoriInZona >= 2)
        {
            triggered = true;
            StartCoroutine(TeleportareNivel());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void IeseZonaServerRpc()
    {
        jucatoriInZona = Mathf.Max(0, jucatoriInZona - 1);
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
        // Schimba nivelele pe fiecare client
        if (nivelCurent != null) nivelCurent.SetActive(false);
        if (nivelUrmator != null) nivelUrmator.SetActive(true);

        // Teleporteaza owner-ul
        PlayerInput[] players = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (player.IsOwner)
                player.transform.position = pozitieNoua;
        }

        // Fade in
        if (SceneFade.Instance != null)
            StartCoroutine(SceneFade.Instance.FadeIn(fadeDuration));
    }
}