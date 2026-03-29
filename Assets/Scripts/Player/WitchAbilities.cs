using UnityEngine;
using Unity.Netcode;

public class WitchAbilities : NetworkBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody2D rb;

    [Header("Setari Glide")]
    public float vitezaCadereLenta = -1f; // cat de incet cade pe Y
    public float multiplicatorVitezaAer = 1.5f; // cu cat inmultim viteza pe X cand planeaza (ex: 1.5 = 50% mai rapida)

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

        // salvam viteza normala de mers 
        vitezaMiscareOriginala = playerInput.vitezaMiscare;
    }

    void FixedUpdate()
    {
        // verifica daca jucatorul pica in jos SI tine apasat pe butonul de sarit
        if (rb.linearVelocity.y < 0 && playerInput.IsJumpHeld)
        {
            // incetinim caderea (Y)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vitezaCadereLenta);
            
            // dam boost de viteza pe orizontala (X)
            if (!planeazaAcum)
            {
                playerInput.vitezaMiscare = vitezaMiscareOriginala * multiplicatorVitezaAer;
                planeazaAcum = true;
            }
        }
        else
        {
            // daca nu mai tine apasat sau a atins pamantul, resetam viteza la normal
            if (planeazaAcum)
            {
                playerInput.vitezaMiscare = vitezaMiscareOriginala;
                planeazaAcum = false;
            }
        }
    }
}