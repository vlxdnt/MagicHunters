//facut special pt animatiile/abilitatile carac Witch

using UnityEngine;

public class WitchAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerInput playerInput;

    void Awake()
    {
        //preluam componentele
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        // pentru mers
        animator.SetFloat("Viteza", Mathf.Abs(playerInput.VectorMiscare.x));

        // setam parametrul pt anim de Floating care merge in Idle
        animator.SetBool("EstePePodea", playerInput.EstePePodea);

        // setam parametru pt anim de JumpPrepare -> midair -> floating
        if (playerInput.AJumped)
            animator.SetTrigger("Sare");
    }
}