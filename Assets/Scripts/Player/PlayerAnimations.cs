using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement movement;
    void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        Debug.Log(movement.IsMoving);

        animator.SetBool("isWalking", movement.IsMoving);
        animator.SetBool("isGrounded", movement.isGrounded);
    }
}
