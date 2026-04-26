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

    private float intervalCautare = 1f;
    private float timerCautare = 0f;

    private Transform currentTarget;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Sincronizăm vizual direcția în care se uită (stânga/dreapta)
    private NetworkVariable<bool> seUitaSpreDreapta = new NetworkVariable<bool>(
        false, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {

        // Dacă ești Client, doar actualizezi direcția vizual și te oprești aici
        if (!IsServer)
        {
            spriteRenderer.flipX = seUitaSpreDreapta.Value;
            return;
        }

        timerCautare -= Time.deltaTime;
        if (timerCautare <= 0f)
        {
            FindClosestPlayer(); // Acum caută jucătorul doar de 2 ori pe secundă, nu de 60 de ori!
            timerCautare = intervalCautare;
        }

        if (currentTarget != null)
        {
            float directionX = currentTarget.position.x - transform.position.x;
            
            // Serverul dictează direcția pentru toată lumea
            seUitaSpreDreapta.Value = (directionX > 0);
            spriteRenderer.flipX = seUitaSpreDreapta.Value;

            if (Time.time >= nextFireTime)
            {
                // Declanșăm animația și pe ecranele clienților
                DeclanseazaAnimatieClientRpc();
                
                StartCoroutine(AttackWithDelay());
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    [ClientRpc]
    void DeclanseazaAnimatieClientRpc()
    {
        // Serverul își rulează animația din corutină, deci dăm trigger doar dacă suntem Client
        if (!IsServer)
        {
            animator.SetTrigger("Attack");
        }
    }

    void FindClosestPlayer()
    {
        // Folosim metoda sigură care ignoră problemele cu inspectorul
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

    IEnumerator AttackWithDelay()
    {
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.3f);

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
            
            // Setăm rotația fireball-ului corect
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            fireball.transform.rotation = Quaternion.Euler(0, 0, angle);

            fireball.GetComponent<NetworkObject>().Spawn();

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