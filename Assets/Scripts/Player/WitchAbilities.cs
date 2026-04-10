using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections; // Necesar pentru IEnumerator (Coroutine)

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

        if (!IsOwner)
        {
            this.enabled = false;
            return;
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