using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerCombat : NetworkBehaviour
{
    [Header("Setari Atac Melee")]
    public Transform punctAtac; 
    public float razaAtac = 0.6f;
    public int damageAtac = 20;
    public float cooldownAtac = 0.5f;
    public LayerMask stratInamici; // Seteaza in Inspector pe layer-ul "Enemy"

    [Header("Setari Knockback")]
    public float fortaKnockbackX = 10f;
    public float fortaKnockbackY = 3f;

    private float timpulUrmatoruluiAtac = 0f;

    private Animator animator;
    private PlayerInput playerInput;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>(); // O preluam
    }

    // Leaga asta la actiunea de Left Click in Player Input
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        if (context.started && Time.time >= timpulUrmatoruluiAtac)
        {
            EfectueazaAtacMelee();
            timpulUrmatoruluiAtac = Time.time + cooldownAtac;
        }
    }

    private void EfectueazaAtacMelee()
    {
        if (animator != null) animator.SetTrigger("Attack");

        float directie = playerInput.flipX.Value ? -1f : 1f;
        punctAtac.localPosition = new Vector3(Mathf.Abs(punctAtac.localPosition.x) * directie, punctAtac.localPosition.y, punctAtac.localPosition.z);

        DeclanseazaAtacServerRpc();
    }

    [ServerRpc]
    private void DeclanseazaAtacServerRpc()
    {
        AplicaAnimatieClientRpc();

        // Serverul detecteaza inamicii
        Collider2D[] inamiciLoviti = Physics2D.OverlapCircleAll(punctAtac.position, razaAtac, stratInamici);

        foreach (Collider2D inamic in inamiciLoviti)
        {
            // Aplicam damage folosind scriptul tau de Health
            Health hp = inamic.GetComponent<Health>();
            if (hp != null) hp.TakeDamage(damageAtac);

            // Aplicam knockback
            Rigidbody2D rbInamic = inamic.GetComponent<Rigidbody2D>();
            if (rbInamic != null)
            {
                float directieX = Mathf.Sign(inamic.transform.position.x - transform.position.x);
                rbInamic.linearVelocity = Vector2.zero; // Resetam viteza curenta pentru consistenta
                rbInamic.AddForce(new Vector2(directieX * fortaKnockbackX, fortaKnockbackY), ForceMode2D.Impulse);
            }
        }
    }

    [ClientRpc]
    private void AplicaAnimatieClientRpc()
    {
        // Ignoram owner-ul ca sa nu ii ruleze animatia de doua ori
        if (IsOwner) return;
        if (animator != null) animator.SetTrigger("Attack");
    }

    void OnDrawGizmosSelected()
    {
        if (punctAtac != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(punctAtac.position, razaAtac);
        }
    }
}