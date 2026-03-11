using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInput : NetworkBehaviour
{
    // Spriteuri
    public Sprite spriteHost;
    public Sprite spriteClient;

    // Variabile pentru controlul miscarii si sariturii
    public float vitezaMiscare = 5f;
    public float fortaSarit = 10f;

    // Variable pentru verificarea daca jucatorul este pe podea
    public Transform verificarePodea;
    public float razaVerificare = 0.2f;
    public LayerMask stratPodea;

    // Componente necesare pentru miscarea si animatia jucatorului
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 vectorMiscare;
    private bool estePePodea;

    // Se apeleaza la crearea obiectului
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        if (OwnerClientId == 0)
        {
            spriteRenderer.sprite = spriteHost;
        }
        else
        {
            spriteRenderer.sprite = spriteClient;
        }
    }
   

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner) return; // Asigura ca doar ownerul poate controla miscarea
        
        // Citeste inputul pentru miscare
        vectorMiscare = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!IsOwner) return; // Asigura ca doar ownerul poate controla saritura
        
        // Verifica daca jucatorul este pe podea inainte de a sari
        if (estePePodea && context.started)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fortaSarit);
        }
    }

    void Update()
    {
        if (!IsOwner) return; // Asigura ca doar ownerul poate controla miscarea

        // Verifica animatorul pentru a seta parametrii de miscare
        if (animator != null)
        {
            animator.SetFloat("Viteza", Mathf.Abs(vectorMiscare.x));
        }

        if (vectorMiscare.x != 0)
        {
            spriteRenderer.flipX = vectorMiscare.x < 0; // Intoarce sprite-ul in functie de directie
        }
    }

    void FixedUpdate()
    {
        // Deseneaza un cerc pentru verificarea daca jucatorul este pe podea
        if (verificarePodea != null)
        {
            estePePodea = Physics2D.OverlapCircle(verificarePodea.position, razaVerificare, stratPodea);
        }

        if (!IsOwner) return; // Asigura ca doar ownerul poate controla miscarea

        // Aplica miscarea pe axa X
        rb.linearVelocity = new Vector2(vectorMiscare.x * vitezaMiscare, rb.linearVelocity.y);
    }
}
