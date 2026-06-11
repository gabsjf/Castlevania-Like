using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement movement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
    }

    public void TriggerJump()
    {
        animator.ResetTrigger("PlayerJump");
        animator.SetTrigger("PlayerJump");
    }

    public void TriggerAttack()
    {
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }

    private void Update()
    {
        animator.SetBool("isWalking", movement.IsMoving);
        animator.SetBool("isGrounded", movement.isGrounded);
    }
}