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
    private float originalMaxSpeed; // Salvam viteza initiala

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
    public float multiplicatorVitezaScut = 0.3f; // Scade la 30% din viteza
    public Color culoareScut = new Color(0.5f, 0.7f, 1f, 1f); // Un albastru deschis/gri
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
        
        // Salvam viteza setata in Inspector la inceput
        if (aiPath != null) originalMaxSpeed = aiPath.maxSpeed;
    }

    void Update()
    {
        if (!IsServer) return;

        // Cautam mereu jucatorul, chiar si cand suntem "busy" (sa-l urmarim cu scutul)
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
        
        // Se poate misca daca are tinta SAU daca e in mijlocul folosirii scutului
        aiPath.canMove = (currentTarget != null);
    }

    IEnumerator FolosesteScut()
    {
        isBusy = true; // Nu poate ataca in acest timp
        nextShieldTime = Time.time + shieldCooldown;

        // 1. Scadem viteza pe Server
        aiPath.maxSpeed = originalMaxSpeed * multiplicatorVitezaScut;
        
        // 2. Activam imunitatea pe Server
        healthScript.isShielded = true;

        // 3. Schimbam culoarea pe toate Clienturile
        SchimbaVizualScutClientRpc(true);

        yield return new WaitForSeconds(shieldDuration);

        // 4. Revenim la normal
        healthScript.isShielded = false;
        aiPath.maxSpeed = originalMaxSpeed;
        SchimbaVizualScutClientRpc(false);
        
        isBusy = false;
    }

    [ClientRpc]
    private void SchimbaVizualScutClientRpc(bool activ)
    {
        if (spriteRenderer == null) return;
        
        // Daca scutul e activ, facem Sprite-ul albastru/gri, altfel revenim la Alb (normal)
        spriteRenderer.color = activ ? culoareScut : Color.white;
    }

    IEnumerator AtacMelee()
    {
        isBusy = true;
        aiPath.canMove = false; // Se opreste complet cand da cu toporul
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