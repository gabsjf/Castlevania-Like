using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;
    private PlayerControl control;
    private Vector2 movement;
    [SerializeField] private float forcaPulo = 5f;
    public bool isGrounded;
    [SerializeField] private int maxJumps = 2;
    private int jumpsRemaining;
    private PlayerAnimations animations;
    public bool IsGrounded => isGrounded;
    public bool IsMoving => Mathf.Abs(movement.x) > 0.1f;
    private SpriteRenderer spriteRenderer;




    void OnJumpPressed(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                forcaPulo
            );

            animations.TriggerJump();

            jumpsRemaining--;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animations = GetComponent<PlayerAnimations>();
        control = new PlayerControl();
        jumpsRemaining = maxJumps;
        spriteRenderer = GetComponent<SpriteRenderer>();
        control.Player.Jump.performed += OnJumpPressed;
    }

    private void OnEnable()
    {
        if (control != null)
            control.Enable();
    }

    private void OnDisable()
    {
        if (control != null)
            control.Disable();
    }


    void Update()
    {
        movement = control.Player.Move.ReadValue<Vector2>();

        if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            movement.x * speed,
            rb.linearVelocity.y
        );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpsRemaining = maxJumps;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
