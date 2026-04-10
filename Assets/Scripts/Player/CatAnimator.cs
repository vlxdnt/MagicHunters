using UnityEngine;

public class CatAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerInput playerInput;
    [Header("Audio Sources")]
    public AudioSource audioFootsteps;  
    public AudioSource audioOneShot;

    [Header("Sunete")]
    public AudioClip sunetSarit;
    public AudioClip sunetDoubleJump;
    public AudioClip sunetAlearga;

    private bool eraInAer = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (!playerInput.IsOwner) return;

        float viteza = Mathf.Abs(playerInput.VectorMiscare.x);
        animator.SetFloat("Viteza", viteza);

        
        // alergat
        if (viteza > 0.1f && playerInput.EstePePodea)
        {
            if (!audioFootsteps.isPlaying)
            {
                audioFootsteps.clip = sunetAlearga;
                audioFootsteps.loop = true;
                audioFootsteps.Play();
            }
        }
        else
        {
            audioFootsteps.Stop();
        }

        if (playerInput.AJumped)
        {
            animator.SetTrigger("Sare");
            audioFootsteps.Stop();

            if (eraInAer)
                audioOneShot.PlayOneShot(sunetDoubleJump);
            else
                audioOneShot.PlayOneShot(sunetSarit);
        }

        eraInAer = !playerInput.EstePePodea;
    }
}