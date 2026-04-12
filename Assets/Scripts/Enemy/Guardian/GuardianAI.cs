using UnityEngine;
using System.Collections;
using Unity.Netcode;
using Pathfinding;

public class GuardianAI : NetworkBehaviour
{
    [Header("A* Pathfinding")]
    public float aggroRadius = 10f;
    public LayerMask playerLayer;
    private AIPath aiPath;
    private AIDestinationSetter destSetter;
    private float originalMaxSpeed; // salvam viteza

    [Header("Atac Melee (Topor)")]
    public float meleeRange = 1.5f;
    public int meleeDamage = 35;
    public float meleeCooldown = 2f;
    public Transform meleeHitbox; 
    public float meleeHitboxRadius = 0.9f;
    private float nextMeleeTime;

    [Header("Abilitate Scut")]
    public float shieldDuration = 2.5f;
    public float shieldCooldown = 7f;
    public float multiplicatorVitezaScut = 0.3f; 
    public Color culoareScut = new Color(0.5f, 0.7f, 1f, 1f);
    private float nextShieldTime;

    private Transform currentTarget;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth healthScript;
    private bool isBusy = false; 

    void Awake()
    {
        aiPath = GetComponent<AIPath>();
        destSetter = GetComponent<AIDestinationSetter>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthScript = GetComponent<EnemyHealth>();
        
        if (aiPath != null) originalMaxSpeed = aiPath.maxSpeed;
    }

    void Update()
    {
        if (!IsServer) return;

        FindClosestPlayer();

        if (isBusy) return;

        if (currentTarget != null)
        {
            float distance = Vector2.Distance(transform.position, currentTarget.position);
            spriteRenderer.flipX = (currentTarget.position.x < transform.position.x);

            if (Time.time >= nextShieldTime)
            {
                StartCoroutine(FolosesteScut());
                return;
            }

            if (distance <= meleeRange && Time.time >= nextMeleeTime)
            {
                StartCoroutine(AtacMelee());
            }
        }
        else
        {
            aiPath.canMove = false;
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
            if (witch != null && witch.esteInvizibil.Value) continue;

            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearest = p.transform;
            }
        }

        currentTarget = nearest;
        destSetter.target = currentTarget;
        
        aiPath.canMove = (currentTarget != null);
    }

    IEnumerator FolosesteScut()
    {
        isBusy = true; 
        nextShieldTime = Time.time + shieldCooldown;

        aiPath.maxSpeed = originalMaxSpeed * multiplicatorVitezaScut;
        
        healthScript.isShielded = true;

        SchimbaVizualScutClientRpc(true);

        yield return new WaitForSeconds(shieldDuration);

        healthScript.isShielded = false;
        aiPath.maxSpeed = originalMaxSpeed;
        SchimbaVizualScutClientRpc(false);
        
        isBusy = false;
    }

    [ClientRpc]
    private void SchimbaVizualScutClientRpc(bool activ)
    {
        if (spriteRenderer == null) return;
        
        spriteRenderer.color = activ ? culoareScut : Color.white;
    }

    IEnumerator AtacMelee()
    {
        isBusy = true;
        aiPath.canMove = false;
        nextMeleeTime = Time.time + meleeCooldown;

        DeclanseazaAnimatieClientRpc("AttackMelee");
        yield return new WaitForSeconds(0.45f);

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(meleeHitbox.position, meleeHitboxRadius, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            Health hp = player.GetComponent<Health>();
            if (hp != null) hp.TakeDamage(meleeDamage);
        }

        yield return new WaitForSeconds(0.5f); 
        isBusy = false;
        aiPath.canMove = true;
    }

    [ClientRpc]
    private void DeclanseazaAnimatieClientRpc(string triggerName)
    {
        if (animator != null) animator.SetTrigger(triggerName);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
        if (meleeHitbox != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meleeHitbox.position, meleeHitboxRadius);
        }
    }
}