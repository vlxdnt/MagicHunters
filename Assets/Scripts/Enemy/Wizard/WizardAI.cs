using UnityEngine;
using System.Collections;

public class WizardAI : MonoBehaviour
{
    [Header("Detection Settings")]
    public float aggroRadius = 5f;
    public LayerMask playerLayer;

    [Header("Combat Settings")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    private float nextFireTime;

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

        // 0.3 second wait time
        yield return new WaitForSeconds(0.3f);

        // Verificăm dacă mai avem țintă (să nu tragă dacă jucătorul a fugit între timp)
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
            Vector2 direction = (currentTarget.position - firePoint.position).normalized;

            fireball.GetComponent<Rigidbody2D>().linearVelocity = direction * 7f;

            fireball.transform.right = direction;

            Destroy(fireball, 3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }
}