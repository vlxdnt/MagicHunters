using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class LevelTransition : NetworkBehaviour
{
    [Header("Setari")]
    public Transform spawnUrmatorNivel; // pozitia de dupa tp

    [Header("Nivele")]
    public GameObject nivelCurent;   // de dezactiv
    public GameObject nivelUrmator;

    public float fadeDuration = 0.5f;

    private int jucatoriInZona = 0;
    private bool triggered = false;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        jucatoriInZona++;
        if (jucatoriInZona >= 2)
        {
            triggered = true;
            StartCoroutine(TeleportareNivel());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jucatoriInZona = Mathf.Max(0, jucatoriInZona - 1);
    }

    IEnumerator TeleportareNivel()
    {
        if (SceneFade.Instance != null)
            yield return StartCoroutine(SceneFade.Instance.FadeOut(fadeDuration));

        // schimba levelu
        if (nivelCurent != null) nivelCurent.SetActive(false);
        if (nivelUrmator != null) nivelUrmator.SetActive(true);

        TeleporteazaClientRpc(spawnUrmatorNivel.position);

        if (SceneFade.Instance != null)
            yield return StartCoroutine(SceneFade.Instance.FadeIn(fadeDuration));
    }

    [ClientRpc]
    void TeleporteazaClientRpc(Vector3 pozitieNoua)
    {
        PlayerInput[] players = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (player.IsOwner)
            {
                player.transform.position = pozitieNoua;

                // next checkp
                PlayerRespawn respawn = player.GetComponent<PlayerRespawn>();
                if (respawn != null)
                    respawn.pozitieRespawn = pozitieNoua;
            }
        }
    }
}