using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

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
    public bool IsJumpHeld { get; private set; }
    public bool controlActiv = false;

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
        // si pt host/client
        flipX.OnValueChanged += OnFlipXChanged;

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

    // se apeleaza automat cand flipX se schimba in retea
    void OnFlipXChanged(bool oldValue, bool newValue)
    {
        if (spriteRenderer != null)
            spriteRenderer.flipX = newValue;
    }

    public override void OnDestroy()
    {
        // dezabonare la destroy ca sa nu avem memory leaks
        flipX.OnValueChanged -= OnFlipXChanged;
    }

    // input system activ la miscare
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner || !controlActiv) return;
        vectorMiscare = context.ReadValue<Vector2>();
    }

    // input system activ la salt
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!IsOwner || !controlActiv) return;
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

    void Update()
    {
        if (!IsOwner) return;

        // doar owner-ul schimba directia 
        if (Mathf.Abs(vectorMiscare.x) > 0.1f)
            flipX.Value = vectorMiscare.x < 0; //true pt stanga
    }

    void LateUpdate()
    {
        AJumped = false;
    }

    void FixedUpdate()
    {
        //pt ground
        if (verificarePodea != null)
            estePePodea = Physics2D.OverlapCircle(verificarePodea.position, razaVerificare, stratPodea);

        //axa X
        if (!IsOwner || !controlActiv) return;
        rb.linearVelocity = new Vector2(vectorMiscare.x * vitezaMiscare, rb.linearVelocity.y);
    }
}