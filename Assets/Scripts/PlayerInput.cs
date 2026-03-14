using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerInput : NetworkBehaviour
{
    public float vitezaMiscare = 5f;
    public float fortaSarit = 10f;

    public Transform verificarePodea;
    public float razaVerificare = 0.2f;
    public LayerMask stratPodea;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 vectorMiscare;
    private bool estePePodea;

    public NetworkVariable<bool> flipX = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            var input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (input != null) input.enabled = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        vectorMiscare = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        if (estePePodea && context.started)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fortaSarit);
        }
    }

    void Update()
    {
        // Flip-ul se aplica pentru toti, nu doar owner
        spriteRenderer.flipX = flipX.Value;

        if (!IsOwner) return;

        if (animator != null)
            animator.SetFloat("Viteza", Mathf.Abs(vectorMiscare.x));

        if (vectorMiscare.x != 0)
            flipX.Value = vectorMiscare.x < 0;
    }

    void FixedUpdate()
    {
        if (verificarePodea != null)
            estePePodea = Physics2D.OverlapCircle(verificarePodea.position, razaVerificare, stratPodea);

        if (!IsOwner) return;
        rb.linearVelocity = new Vector2(vectorMiscare.x * vitezaMiscare, rb.linearVelocity.y);
    }
}