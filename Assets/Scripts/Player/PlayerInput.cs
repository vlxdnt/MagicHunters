using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using NUnit.Framework;

// pentru input/miscare comuna a jucatorilor
// sau individual prin WitchAnimator.cs/CatAnimator.cs
public class PlayerInput : NetworkBehaviour
{
    [Header("Miscare")]
    public float vitezaMiscare = 1f;
    public float fortaSarit = 5f;

    [Header("Verificare Podea")]
    public Transform verificarePodea;
    public float razaVerificare = 0.2f;
    public LayerMask stratPodea;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 vectorMiscare;
    private bool estePePodea;

    // parametrii
    public bool EstePePodea => estePePodea;
    public Vector2 VectorMiscare => vectorMiscare;
    public bool AJumped { get; private set; }
    public bool IsJumpHeld {get; private set;}

    //flip sincronizat
    public NetworkVariable<bool> flipX = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // obiectul e spawnat in retea
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            var input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (input != null) input.enabled = false;

            // oprire camera
            Camera cameraJucator = GetComponentInChildren<Camera>();
            if (cameraJucator != null)
                cameraJucator.gameObject.SetActive(false);

            AudioListener otherEars = GetComponentInChildren<AudioListener>();
            if (otherEars != null) otherEars.enabled = false;
        }
    }

    // input system activ la miscare
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        vectorMiscare = context.ReadValue<Vector2>();
    }

    // input system activ la salt
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        if (context.started)
        {
            if (estePePodea)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, fortaSarit);
                AJumped = true; // semnalizam ca a sarit
            }
            IsJumpHeld = true; // semnalizam ca tasta de salt e tinuta
        }
        else if (context.canceled)
        {
            IsJumpHeld = false; // semnalizam ca tasta de salt nu mai e tinuta
        }
    }

    void Update() //de verificat
    {
        // flip vizual
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = flipX.Value;
        }

        if (!IsOwner) return;

        if (Mathf.Abs(vectorMiscare.x) > 0.1f)
        {
            flipX.Value = vectorMiscare.x < 0; //true pt stanga
        }
    }

    void LateUpdate()
    {
        AJumped = false;
    }

    void FixedUpdate()
    {
        // verificare podea
        if (verificarePodea != null)
            estePePodea = Physics2D.OverlapCircle(verificarePodea.position, razaVerificare, stratPodea);

        if (!IsOwner) return;

        // miscarea pe axa X
        rb.linearVelocity = new Vector2(vectorMiscare.x * vitezaMiscare, rb.linearVelocity.y);
    }
}