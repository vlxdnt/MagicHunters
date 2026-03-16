using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

// Gestioneaza inputul si miscarea jucatorului
// Foloseste NetworkBehaviour pentru sincronizare in retea
public class PlayerInput : NetworkBehaviour
{
    [Header("Miscare")]
    public float vitezaMiscare = 1f;
    public float fortaSarit = 5f;

    [Header("Verificare Podea")]
    public Transform verificarePodea; // Punctul de unde verificam daca suntem pe podea
    public float razaVerificare = 0.2f;
    public LayerMask stratPodea;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 vectorMiscare;
    private bool estePePodea;

    // NetworkVariable pentru flip sincronizat - doar ownerul poate scrie, toti pot citi
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

    // Se apeleaza cand obiectul e spawnat in retea
    public override void OnNetworkSpawn()
    {
        // Dezactivam inputul pentru jucatorii care nu sunt ownerul acestui obiect
        if (!IsOwner)
        {
            var input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (input != null) input.enabled = false;

            // Oprim camera pentru personajul celalalt, ca sa nu ne fure ecranul
            Camera cameraJucator = GetComponentInChildren<Camera>();
            if (cameraJucator != null)
            {
                cameraJucator.gameObject.SetActive(false);
            }
        }
    }

    // Apelata de Input System cand jucatorul misca (WASD / Sageti)
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        vectorMiscare = context.ReadValue<Vector2>();
    }

    // Apelata de Input System cand jucatorul sare (Space)
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
        // Aplicam flip-ul pentru toti jucatorii (si non-owner) ca sa se vada corect
        spriteRenderer.flipX = flipX.Value;

        if (!IsOwner) return;

        // Actualizam animatia de mers
        if (animator != null)
            animator.SetFloat("Viteza", Mathf.Abs(vectorMiscare.x));

        // Sincronizam directia sprite-ului prin retea
        if (vectorMiscare.x != 0)
            flipX.Value = vectorMiscare.x < 0; // true = stanga, false = dreapta
    }

    void FixedUpdate()
    {
        // Verificam daca jucatorul e pe podea folosind un cerc la picioare
        if (verificarePodea != null)
            estePePodea = Physics2D.OverlapCircle(verificarePodea.position, razaVerificare, stratPodea);

        if (!IsOwner) return;

        // Aplicam miscarea pe axa X pastr�nd viteza verticala
        rb.linearVelocity = new Vector2(vectorMiscare.x * vitezaMiscare, rb.linearVelocity.y);
    }
}