using UnityEngine;

public class WitchAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerInput playerInput;
    [Header("Audio Sources")]
    public AudioSource audioFootsteps;
    public AudioSource audioOneShot;

    [Header("Sunete")]
    public AudioClip sunetSarit;
    public AudioClip sunetAlearga;

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
        animator.SetBool("EstePePodea", playerInput.EstePePodea);

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
            audioOneShot.PlayOneShot(sunetSarit);
        }
    }
}