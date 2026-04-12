using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class BossAI : NetworkBehaviour
{
    [Header("Detection")]
    public float aggroRadius = 20f; // raza mare
    public LayerMask playerLayer;

    [Header("Abilități - Fireball")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireRate = 3f;
    private float nextFireTime;

    [Header("Abilități - Scut (Block)")]
    public float shieldDuration = 3f;
    public float shieldCooldown = 10f;
    public Color culoareScut = new Color(0.3f, 0.4f, 1f, 1f);
    private float nextShieldTime;

    [Header("Abilități - Spawn Minions")]
    public GameObject wizardPrefab;
    public GameObject batPrefab;
    public Transform[] spawnPoints;
    public float spawnCooldown = 15f;
    public int maxActiveMinions = 5;
    private float nextSpawnTime;

    private List<NetworkObject> minioniSummonati = new List<NetworkObject>();

    private Transform currentTarget;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth healthScript;
    private bool isBusy = false; 

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthScript = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (!IsServer) return;

        FindClosestPlayer();

        if (isBusy) return; // in timpul unui atac/scut

        if (currentTarget != null)
        {
            float distance = Vector2.Distance(transform.position, currentTarget.position);

            // se intoarce cu fata la jucator
            spriteRenderer.flipX = (currentTarget.position.x < transform.position.x);

            if (Time.time >= nextShieldTime && distance <= 8f)
            {
                StartCoroutine(FolosesteScut());
                return;
            }

            if (Time.time >= nextSpawnTime)
            {
                StartCoroutine(SpawnMinions());
                return;
            }

            if (Time.time >= nextFireTime)
            {
                StartCoroutine(AttackFireball());
            }
        }
    }

    void FindClosestPlayer()
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, aggroRadius, playerLayer);
        float shortestDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (Collider2D p in players)
        {
            WitchAbilities witch = p.GetComponent<WitchAbilities>();
            if (witch != null && witch.esteInvizibil.Value == true) continue;

            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearest = p.transform;
            }
        }
        currentTarget = nearest;
    }

    IEnumerator FolosesteScut()
    {
        isBusy = true; 
        nextShieldTime = Time.time + shieldCooldown;

        TriggeazaVizualScutClientRpc(true);
        healthScript.isShielded = true; 

        yield return new WaitForSeconds(shieldDuration);

        healthScript.isShielded = false;
        TriggeazaVizualScutClientRpc(false);
        isBusy = false;
    }

    [ClientRpc]
    private void TriggeazaVizualScutClientRpc(bool activ)
    {
        if (animator != null) animator.SetBool("IsBlocking", activ); 
        if (spriteRenderer != null) spriteRenderer.color = activ ? culoareScut : Color.white;
    }

    IEnumerator AttackFireball()
    {
        isBusy = true;
        nextFireTime = Time.time + fireRate;

        TriggerAnimatieClientRpc("AttackFireball");
        yield return new WaitForSeconds(0.4f);

        if (fireballPrefab != null && firePoint != null && currentTarget != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            
            Vector3 targetPos = currentTarget.position + new Vector3(0, 0.8f, 0);
            Vector2 dir = (targetPos - firePoint.position).normalized;
            
            fireball.GetComponent<Rigidbody2D>().linearVelocity = dir * 9f;
            fireball.GetComponent<NetworkObject>().Spawn();
            
            StartCoroutine(DespawnDupaTimp(fireball, 4f));
        }

        yield return new WaitForSeconds(0.6f); 
        isBusy = false;
    }

    IEnumerator SpawnMinions()
    {
        CurataListaMinioni();

        if (minioniSummonati.Count >= maxActiveMinions)
        {
            nextSpawnTime = Time.time + 3f; 
            yield break; 
        }

        isBusy = true;
        nextSpawnTime = Time.time + spawnCooldown;

        TriggerAnimatieClientRpc("SpawnMinions");
        yield return new WaitForSeconds(1f); 

        int randomSpawn = Random.Range(0, 2); 

        if (randomSpawn == 0 && wizardPrefab != null)
        {
            SummonMinion(wizardPrefab);
        }
        else if (batPrefab != null)
        {
            SummonMinion(batPrefab);
            yield return new WaitForSeconds(0.3f);
            SummonMinion(batPrefab);
        }

        yield return new WaitForSeconds(0.5f);
        isBusy = false;
    }

    private void SummonMinion(GameObject prefab)
    {
        if (minioniSummonati.Count >= maxActiveMinions) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject minion = Instantiate(prefab, sp.position, Quaternion.identity);
        
        NetworkObject netObj = minion.GetComponent<NetworkObject>();
        netObj.Spawn();
        minioniSummonati.Add(netObj);
    }

    private void CurataListaMinioni()
    {
        for (int i = minioniSummonati.Count - 1; i >= 0; i--)
        {
            if (minioniSummonati[i] == null || !minioniSummonati[i].IsSpawned)
            {
                minioniSummonati.RemoveAt(i);
            }
        }
    }

    [ClientRpc]
    private void TriggerAnimatieClientRpc(string triggerName)
    {
        if (animator != null) animator.SetTrigger(triggerName);
    }

    private IEnumerator DespawnDupaTimp(GameObject obj, float timp)
    {
        yield return new WaitForSeconds(timp);
        if (obj != null)
        {
            var netObj = obj.GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsSpawned) netObj.Despawn();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
        
        if (firePoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(firePoint.position, 0.3f);
        }

        if (spawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var sp in spawnPoints)
            {
                if (sp != null) Gizmos.DrawWireSphere(sp.position, 0.5f);
            }
        }
    }
}