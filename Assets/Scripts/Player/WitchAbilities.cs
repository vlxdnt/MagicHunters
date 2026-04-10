using UnityEngine;
using Unity.Netcode;

public class WitchAbilities : NetworkBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    [Header("Setari Glide")]
    public float vitezaCadereLenta = -1f;
    public float multiplicatorVitezaAer = 1.5f;

    [Header("Sunete")]
    public AudioClip sunetGlide;
    [Header("Audio")]
    public AudioSource audioOneShot;

    private float vitezaMiscareOriginala;
    private bool planeazaAcum = false;

    public override void OnNetworkSpawn()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();

        if (!IsOwner)
        {
            this.enabled = false;
            return;
        }

        vitezaMiscareOriginala = playerInput.vitezaMiscare;
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        // glide
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