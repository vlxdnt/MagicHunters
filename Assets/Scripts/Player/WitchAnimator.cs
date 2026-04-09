using UnityEngine;

public class WitchAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerInput playerInput;
    private AudioSource audioSource;

    [Header("Sunete")]
    public AudioClip sunetSarit;
    public AudioClip sunetAlearga;

    [Header("Footstep Settings")]
    public float footstepInterval = 0.3f;
    private float footstepTimer = 0f;

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
        animator.SetBool("EstePePodea", playerInput.EstePePodea);

        // jump
        if (playerInput.AJumped)
        {
            animator.SetTrigger("Sare");
            PlaySound(sunetSarit);
        }

        // pasi
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