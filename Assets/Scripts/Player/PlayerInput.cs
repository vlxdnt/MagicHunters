using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections; // ---> ADAUGAT pentru Corutine

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
    
    private bool dorintaSarit = false; 

    public bool EstePePodea => estePePodea;
    public Vector2 VectorMiscare => vectorMiscare;
    public bool AJumped { get; private set; }
    public bool ADashed { get; private set; }
    public bool miscareBlocata = false;
    public bool IsJumpHeld { get; private set; }
    public bool controlActiv = false;

    // ---> ADAUGAT PENTRU PLATFORME <---
    private Collider2D playerCollider; 
    private GameObject platformaCurenta; 
    private bool trecePrinPlatforma = false;
    // ----------------------------------

    public NetworkVariable<bool> flipX = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>(); // ---> ADAUGAT
    }

    public override void OnNetworkSpawn()
    {
        flipX.OnValueChanged += OnFlipXChanged;

        if (!IsOwner)
        {
            var input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (input != null) input.enabled = false;

            Camera cameraMea = GetComponentInChildren<Camera>();
            if (cameraMea != null) cameraMea.tag = "MainCamera";
            
            Camera cameraJucator = GetComponentInChildren<Camera>();
            if (cameraJucator != null) cameraJucator.gameObject.SetActive(false);

            AudioListener otherEars = GetComponentInChildren<AudioListener>();
            if (otherEars != null) otherEars.enabled = false;
        }
    }

    void OnFlipXChanged(bool oldValue, bool newValue)
    {
        if (spriteRenderer != null)
            spriteRenderer.flipX = newValue;
    }

    public override void OnDestroy()
    {
        flipX.OnValueChanged -= OnFlipXChanged;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner || !controlActiv) return;
        vectorMiscare = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!IsOwner || !controlActiv) return;
        
        if (context.started)
        {
            dorintaSarit = true; 
            IsJumpHeld = true; 
        }
        else if (context.canceled)
        {
            IsJumpHeld = false; 
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!IsOwner || !controlActiv) return; 
        if (context.started) ADashed = true;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Mathf.Abs(vectorMiscare.x) > 0.1f)
        {
            bool trebuieSaFieStanga = vectorMiscare.x < 0;
            
            if (flipX.Value != trebuieSaFieStanga)
            {
                flipX.Value = trebuieSaFieStanga; 
            }
        }

        // ---> ADAUGAT: Logica de a cadea prin platforma <---
        // Daca jucatorul tine apasat in jos (S), se afla pe o platforma si nu a activat deja caderea
        if (vectorMiscare.y <= -0.5f && estePePodea && platformaCurenta != null && !trecePrinPlatforma)
        {
            // Ne asiguram ca e o platforma de tip "One Way" (ca sa nu cazi prin podeaua principala)
            if (platformaCurenta.GetComponent<PlatformEffector2D>() != null)
            {
                StartCoroutine(RutinaTrecerePlatforma(platformaCurenta.GetComponent<Collider2D>()));
            }
        }
        // ---------------------------------------------------
    }

    // ---> ADAUGAT: Corutina care opreste coliziunea 0.5 secunde <---
    private IEnumerator RutinaTrecerePlatforma(Collider2D colliderPlatforma)
    {
        trecePrinPlatforma = true;

        // Ignoram coliziunea intre jucatorul nostru si platforma (celalalt jucator va putea sta pe ea in continuare)
        Physics2D.IgnoreCollision(playerCollider, colliderPlatforma, true);

        // Asteptam suficient cat sa cada prin ea
        yield return new WaitForSeconds(0.4f);

        // Reactivam coliziunea
        Physics2D.IgnoreCollision(playerCollider, colliderPlatforma, false);
        trecePrinPlatforma = false;
    }
    // ---------------------------------------------------------------

    void LateUpdate()
    {
        AJumped = false;
        ADashed = false;
    }

    void FixedUpdate()
    {
        if (!IsOwner) return; 

        if (verificarePodea != null)
        {
            Collider2D obiectLovit = Physics2D.OverlapBox(verificarePodea.position, dimensiuneVerificare, 0f, stratPodea);
            estePePodea = obiectLovit != null;

            // ---> ADAUGAT: Retinem pe ce obiect stam <---
            platformaCurenta = obiectLovit != null ? obiectLovit.gameObject : null;

            if (estePePodea && rb.linearVelocity.y <= 0.01f)
            {
                sarituriRamase = sarituriMaxime; 
            }
        }

        if (dorintaSarit)
        {
            if (sarituriRamase > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); 
                rb.AddForce(Vector2.up * fortaSarit, ForceMode2D.Impulse); 
                
                AJumped = true; 
                sarituriRamase--; 
            }
            dorintaSarit = false; 
        }

        if (!miscareBlocata) 
        {
            rb.linearVelocity = new Vector2(vectorMiscare.x * vitezaMiscare, rb.linearVelocity.y);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (verificarePodea != null)
        {
            Gizmos.color = estePePodea ? Color.green : Color.red;
            Gizmos.DrawWireCube(verificarePodea.position, dimensiuneVerificare);
        }
    }
}