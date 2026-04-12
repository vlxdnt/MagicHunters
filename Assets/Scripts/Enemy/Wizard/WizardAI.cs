using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class WizardAI : NetworkBehaviour
{
    [Header("Detection Settings")]
    public float aggroRadius = 5f;
    public LayerMask playerLayer;

    [Header("Combat Settings")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    private float nextFireTime;
    private float feetOffset = 0.3f;

    private Transform currentTarget;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!IsServer) return;
        FindClosestPlayer();

        if (currentTarget != null)
        {
            float directionX = currentTarget.position.x - transform.position.x;

            spriteRenderer.flipX = (directionX > 0);

            if (Time.time >= nextFireTime)
            {
                StartCoroutine(AttackWithDelay());
                nextFireTime = Time.time + fireRate;
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
            if (witch != null && witch.esteInvizibil.Value == true)
            {
                continue; // next jucator
            }

            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearest = p.transform;
            }
        }
        currentTarget = nearest;
    }

    IEnumerator AttackWithDelay()
    {
        animator.SetTrigger("Attack");

        // 0.3 wait time
        yield return new WaitForSeconds(0.3f);

        // daca mai avem tinta
        if (currentTarget != null)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (fireballPrefab != null && firePoint != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

            Vector3 pozitieTinta = currentTarget.position + new Vector3(0f, feetOffset, 0f);
            
            Vector2 direction = (pozitieTinta - firePoint.position).normalized;
            fireball.GetComponent<Rigidbody2D>().linearVelocity = direction * 7f;
            fireball.transform.right = direction;

            //spawn pe retea
            fireball.GetComponent<NetworkObject>().Spawn();

            //despawn dupa un timp
            StartCoroutine(DespawnDupaTimp(fireball, 3f));
        }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }
}