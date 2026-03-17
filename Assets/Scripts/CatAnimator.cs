//facut pt animatii/abilitati viitoare

using UnityEngine;

public class CatAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerInput playerInput;

    void Awake()
    {
        //preluare
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (!playerInput.IsOwner) return;

        // pt animatia de mers (setam parametrul)
        animator.SetFloat("Viteza", Mathf.Abs(playerInput.VectorMiscare.x));

        // pt anim de sarit
        if (playerInput.AJumped)
            animator.SetTrigger("Sare");

        // parametrii pe viitor
    }
}