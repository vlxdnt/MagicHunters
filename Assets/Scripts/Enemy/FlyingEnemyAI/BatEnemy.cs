using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using Unity.Netcode;

public class BatEnemy : NetworkBehaviour
{
    [Header("Setari Atac")]
    public float forțaKnockback = 10f;
    public float cooldownKnockback = 1f;
    public LayerMask playerLayer;
    public int damageLaAtac = 10;
    
    [Header("Setari Detectie (Nou)")]
    public float razaDetectie = 12f; // Cat de departe vede jucatorii

    private AIDestinationSetter destinationSetter;
    private AIPath aiPath;
    private float nextKnockbackTime = 0f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sunetAtacLiliac;

    // timer pt fps fix
    private float intervalCautare = 0.5f;
    private float timerCautare = 0f;

    void Awake()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();

        // Il tinem pe loc pana cand reteaua e gata
        aiPath.canMove = false;
    }

    public override void OnNetworkSpawn()
    {
        // SALVAREA FPS-URILOR: Oprim Pathfinding-ul pe client!
        if (!IsServer)
        {
            aiPath.enabled = false;
            destinationSetter.enabled = false;
        }
    }

    void Update()
    {
        // Doar serverul gandeste
        if (!IsSpawned || !IsServer) return;

        // Cautam jucatorul de 2 ori pe secunda
        timerCautare -= Time.deltaTime;
        if (timerCautare <= 0f)
        {
            ActualizeazaTargetRadar();
            timerCautare = intervalCautare;
        }
    }

    // Am inlocuit JucatorIntrat/Iesit cu radarul asta sigur pentru Multiplayer
    void ActualizeazaTargetRadar()
    {
        Collider2D[] jucatoriInZona = Physics2D.OverlapCircleAll(transform.position, razaDetectie, playerLayer);
        
        float distantaMinima = Mathf.Infinity;
        Transform targetNou = null;

        foreach (var hit in jucatoriInZona)
        {
            if (hit.CompareTag("Player"))
            {
                WitchAbilities witch = hit.GetComponent<WitchAbilities>();
                // Daca e vrajitoare si e invizibila, o ignoram
                if (witch != null && witch.esteInvizibil.Value) continue;

                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < distantaMinima)
                {
                    distantaMinima = dist;
                    targetNou = hit.transform;
                }
            }
        }

        destinationSetter.target = targetNou;
        aiPath.canMove = (targetNou != null);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Doar serverul are voie sa calculeze damage-ul, altfel loviti de 2 ori
        if (!IsServer || Time.time < nextKnockbackTime) return;
        if (!other.CompareTag("Player")) return;

        JoacaSunetClientRpc(); // Spunem tuturor sa redea sunetul

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float directieX = Mathf.Sign(other.transform.position.x - transform.position.x);
            Vector2 directie = new Vector2(directieX, 0.3f).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(directie * forțaKnockback, ForceMode2D.Impulse);
        }

        Health health = other.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damageLaAtac);

        nextKnockbackTime = Time.time + cooldownKnockback;
    }

    // Trimitem sunetul prin retea ca sa il auda si celalalt jucator
    [ClientRpc]
    void JoacaSunetClientRpc()
    {
        if (audioSource != null && sunetAtacLiliac != null)
            audioSource.PlayOneShot(sunetAtacLiliac);
    }

    // Ca sa poti regla raza in Unity Editor mai usor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, razaDetectie);
    }
}