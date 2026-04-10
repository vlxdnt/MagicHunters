using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class CatAbilities : NetworkBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody2D rb;

    [Header("Setari Dash")]
    public float fortaDash = 15f;   
    public float ridicareDash = 2f; 
    public float durataDash = 0.2f;  
    public float cooldownDash = 2f;  

    private bool isDashing = false;
    private bool canDash = true;

    private AudioSource audioSource;
    [Header("Sunete")]
    public AudioClip sunetDash;
    public AudioSource audioOneShot;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (playerInput.ADashed && canDash && !isDashing)
        {
            StartCoroutine(EfectueazaDash());
        }
    }

    IEnumerator EfectueazaDash()
    {
        isDashing = true;
        canDash = false;

        // blocam miscarea normala
        if (sunetDash != null && audioOneShot != null)
            audioOneShot.PlayOneShot(sunetDash);

        playerInput.miscareBlocata = true;

        // oprim gravitatia ca sa facem un dash curat (liniar)
        float gravitateOriginala = rb.gravityScale;
        rb.gravityScale = 0f;

        // determinam directia (stanga sau dreapta) pe baza variabilei flipX din PlayerInput
        float directieX = playerInput.flipX.Value ? -1f : 1f;

        // aplicam forta
        rb.linearVelocity = new Vector2(directieX * fortaDash, ridicareDash);

        // asteptam cat dureaza dash-ul
        yield return new WaitForSeconds(durataDash);

        // oprim dash-ul si redam controlul
        rb.gravityScale = gravitateOriginala;
        playerInput.miscareBlocata = false;
        isDashing = false;

        // asteptam cooldown-ul inainte sa ii dam voie iar
        yield return new WaitForSeconds(cooldownDash);
        canDash = true;
    }
}