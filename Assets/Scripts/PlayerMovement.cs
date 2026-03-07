using Unity.Netcode;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    [Header("Miscare si Saritura")]
    public float viteza = 5f;
    public float jumpForce = 10f;

    [Header("Detectare Podea")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!IsOwner) return;

        float moveX = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(moveX * viteza, rb.linearVelocity.y);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveX));
        }

        if (moveX != 0)
        {
            spriteRenderer.flipX = moveX < 0;
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
    }
}