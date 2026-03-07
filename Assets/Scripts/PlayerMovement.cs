using Unity.Netcode;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    public float viteza = 5f;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

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
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 miscare = new Vector2(moveX, moveY).normalized;

        rb.linearVelocity = miscare * viteza;

        if (anim != null)
        {
            anim.SetFloat("Speed", miscare.magnitude);
        }

        if (moveX != 0)
        {
            spriteRenderer.flipX = moveX < 0;
        }
    }
}