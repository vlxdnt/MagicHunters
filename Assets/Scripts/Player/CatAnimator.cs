using UnityEngine;

public class CatAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerInput playerInput;
    private AudioSource audioSource;

    [Header("Sunete")]
    public AudioClip sunetSarit;
    public AudioClip sunetDoubleJump;
    public AudioClip sunetAlearga;

    [Header("Footstep Settings")]
    public float footstepInterval = 0.3f;
    private float footstepTimer = 0f;
    private bool eraInAer = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!playerInput.IsOwner) return;

        float viteza = Mathf.Abs(playerInput.VectorMiscare.x);
        animator.SetFloat("Viteza", viteza);

        // Jump / Double Jump
        if (playerInput.AJumped)
        {
            animator.SetTrigger("Sare");
            if (eraInAer)
                PlaySound(sunetDoubleJump); // al doilea salt
            else
                PlaySound(sunetSarit); // primul salt
        }

        // retinem daca era in aer inainte
        eraInAer = !playerInput.EstePePodea;

        // footsteps
        if (viteza > 0.1f && playerInput.EstePePodea)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlaySound(sunetAlearga);
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = footstepInterval;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}