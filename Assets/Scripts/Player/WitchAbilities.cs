using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections;

public class WitchAbilities : NetworkBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [Header("Setari Glide")]
    public float vitezaCadereLenta = -1f;
    public float multiplicatorVitezaAer = 1.5f;

    [Header("Sunete")]
    public AudioClip sunetGlide;
    [Header("Audio")]
    public AudioSource audioOneShot;

    [Header("Setari Invizibilitate")]
    public float durataInvizibilitate = 5f;
    public float cooldownInvizibilitate = 8f;
    private bool abilitateInCooldown = false;
    public static System.Collections.Generic.List<WitchAbilities> jucatoriInScena = new System.Collections.Generic.List<WitchAbilities>();

    [Header("Setari Heal")]
    public int cantitateHeal = 50;
    public float cooldownHeal = 10f;
    private bool healInCooldown = false;

    [Header("Setari Fireball")]
    public GameObject fireballPrefab;
    public Transform punctSpawnFireball;
    public float vitezaFireball = 7f; 
    public float timpViataFireball = 3f;
    public float cooldownFireball = 1.5f;
    private float timpUrmatorFireball = 0f;

    [Header("Audio Fireball")]
    public AudioClip sunetLansareFireball;

    private float vitezaMiscareOriginala;
    private bool planeazaAcum = false;

    public NetworkVariable<bool> esteInvizibil = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        esteInvizibil.OnValueChanged += OnInvizibilitateSchimbata;

        if (IsOwner)
        {
            vitezaMiscareOriginala = playerInput.vitezaMiscare;
            if (!jucatoriInScena.Contains(this))
                jucatoriInScena.Add(this);
        }

        vitezaMiscareOriginala = playerInput.vitezaMiscare;
        jucatoriInScena.Add(this);
    }

    public override void OnNetworkDespawn()
    {
        esteInvizibil.OnValueChanged -= OnInvizibilitateSchimbata;
        jucatoriInScena.Remove(this);
    }

    private void OnInvizibilitateSchimbata(bool oldValue, bool newValue)
    {
        if (spriteRenderer != null)
        {
            Color culoare = spriteRenderer.color;
            culoare.a = newValue ? 0.3f : 1f; 
            spriteRenderer.color = culoare;
        }
    }

    public void OnInvizibilitate(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        if (!playerInput.controlActiv) return; // pt cutscene

        if (context.started && !abilitateInCooldown)
        {
            StartCoroutine(RutinaInvizibilitate());
        }
    }

    private IEnumerator RutinaInvizibilitate()
    {
        abilitateInCooldown = true;
        esteInvizibil.Value = true;

        yield return new WaitForSeconds(durataInvizibilitate);

        esteInvizibil.Value = false; 

        yield return new WaitForSeconds(cooldownInvizibilitate - durataInvizibilitate);
        abilitateInCooldown = false;
    }

    public void OnHeal(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        if (context.started)
        {
            if (!healInCooldown)
            {
                StartCoroutine(RutinaHealCooldown());
                DeclanseazaHealServerRpc();
            }
        }
    }

    private IEnumerator RutinaHealCooldown()
    {
        healInCooldown = true;
        yield return new WaitForSeconds(cooldownHeal);
        healInCooldown = false;
    }

    [ServerRpc]
    private void DeclanseazaHealServerRpc()
    {
        AplicaHealClientRpc();
    }

    [ClientRpc]
    private void AplicaHealClientRpc()
    {
        PlayerInput[] totiJucatorii = Object.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);

        foreach (PlayerInput player in totiJucatorii)
        {
            Health hp = player.GetComponent<Health>();
            if (hp != null)
            {
                hp.Heal(cantitateHeal);
            }
        }
    }

    public void OnFireball(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        if (!playerInput.controlActiv) return; // pt cutscene

        if (context.started && Time.time >= timpUrmatorFireball)
        {
            Animator anim = GetComponentInChildren<Animator>();
            if (anim != null) anim.SetTrigger("Attack");

            bool privesteStanga = playerInput.flipX.Value;
            SpawnFireballServerRpc(privesteStanga);
            timpUrmatorFireball = Time.time + cooldownFireball;
        }
    }

    [ServerRpc]
    private void SpawnFireballServerRpc(bool privesteStanga)
    {
        GameObject fireball = Instantiate(fireballPrefab, punctSpawnFireball.position, Quaternion.identity);

        if (audioOneShot != null && sunetLansareFireball != null)
            audioOneShot.PlayOneShot(sunetLansareFireball);

        // directia ca la wizard
        float directieX = privesteStanga ? -1f : 1f;
        Vector2 direction = new Vector2(directieX, 0f);
        fireball.GetComponent<Rigidbody2D>().linearVelocity = direction * vitezaFireball;
        
        fireball.transform.right = direction;

        fireball.GetComponent<NetworkObject>().Spawn();

        StartCoroutine(DespawnFireballRoutine(fireball.GetComponent<NetworkObject>()));
    }

    private System.Collections.IEnumerator DespawnFireballRoutine(NetworkObject netObj)
    {
        //3 sec
        yield return new WaitForSeconds(timpViataFireball);
        
        // il scoatem din joc
        if (netObj != null && netObj.IsSpawned)
        {
            netObj.Despawn();
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (rb.linearVelocity.y < 0 && playerInput.IsJumpHeld && !playerInput.EstePePodea)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vitezaCadereLenta);

            if (!planeazaAcum)
            {
                playerInput.vitezaMiscare = vitezaMiscareOriginala * multiplicatorVitezaAer;
                planeazaAcum = true;

                if (sunetGlide != null && audioOneShot != null)
                {
                    audioOneShot.clip = sunetGlide;
                    audioOneShot.loop = true;
                    audioOneShot.PlayOneShot(sunetGlide);
                }
            }
        }
        else
        {
            if (planeazaAcum)
            {
                playerInput.vitezaMiscare = vitezaMiscareOriginala;
                planeazaAcum = false;

                if (audioOneShot.clip == sunetGlide)
                {
                    audioOneShot.Stop();
                    audioOneShot.loop = false;
                }
            }
        }
    }
}