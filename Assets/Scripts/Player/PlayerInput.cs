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
    public int sarituriMaxime = 1;
    private int sarituriRamase;


    [Header("Verificare Podea")]
    public Transform verificarePodea;
    public Vector2 dimensiuneVerificare = new Vector2(0.5f, 0.1f);
    public LayerMask stratPodea;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 vectorMiscare;
    private bool estePePodea;

    // parametrii
    public bool EstePePodea => estePePodea;
    public Vector2 VectorMiscare => vectorMiscare;
    public bool AJumped { get; private set; }
    public bool ADashed { get; private set; }
    public bool miscareBlocata = false;
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
            if (sarituriRamase > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); 
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, fortaSarit);
                AJumped = true; // semnalizam ca a sarit
                IsJumpHeld = true; // semnalizam ca tasta de salt e tinuta
                sarituriRamase--; // scadem sariturile ramase
            }
        }
        else if (context.canceled)
        {
            IsJumpHeld = false; // semnalizam ca tasta de salt nu mai e tinuta
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        if (context.started)
        {
            ADashed = true; // semnalizam ca a dat dash
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
        ADashed = false;
    }

    void FixedUpdate()
    {
        //pt ground
        if (verificarePodea != null)
        {
            Collider2D obiectLovit = Physics2D.OverlapBox(verificarePodea.position, dimensiuneVerificare, 0f, stratPodea);
            estePePodea = obiectLovit != null;

            // NU STERGE IFUL ASTA, SOMEHOW LOGUL ASTA A REPARAT JUMPUL, NU STIU DE CE, DAR DACA IL STERGI PROBABIL SA SE STRICE
            if (estePePodea)
            {
                Debug.Log($"[{gameObject.name}] Cutia a lovit: " + obiectLovit.gameObject.name);
            }

            if (estePePodea && rb.linearVelocity.y <= 0.1f)
            {
                sarituriRamase = sarituriMaxime; // resetam sariturile cand aterizeaza
            }
        }
        
        //axa X
        if (!IsOwner || miscareBlocata) return;
        rb.linearVelocity = new Vector2(vectorMiscare.x * vitezaMiscare, rb.linearVelocity.y);
    }

    // DEBUG
    void OnDrawGizmosSelected()
    {
        if (verificarePodea != null)
        {
            // seteaza culoarea cutiei
            Gizmos.color = estePePodea ? Color.green : Color.red;

            // deseneaza cutia exacta pe care o foloseste fizica
            // folosim dimensiunea setata de tine in Inspector
            Gizmos.DrawWireCube(verificarePodea.position, dimensiuneVerificare);
        }
    }
}