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
    public bool tomouKnockback = false;

    [Header("Sistema do Void")]
    public Vector3 ultimaPosicaoSegura;
    private float tempoNoChao = 0f;
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
        control.Player.Attack.performed += OnAttackPressed;
    }

    private void OnAttackPressed(InputAction.CallbackContext context)
    {
        Debug.Log("ATAQUE!");
        animations.TriggerAttack();
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
            
        // Garante que o player pare a animação de andar se for desativado
        movement = Vector2.zero; 
    }


    void Update()
    {
        movement = control.Player.Move.ReadValue<Vector2>();

        if (movement.x > 0)
        {
            // Gira o personagem inteiro (e o AttackPoint) para a Direita
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (movement.x < 0)
        {
            // Gira o personagem inteiro (e o AttackPoint) para a Esquerda
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (isGrounded)
        {
            tempoNoChao += Time.deltaTime;
            // Se o player ficar parado ou correndo no chão por mais de meio segundo, salva o ponto
            if (tempoNoChao > 0.5f)
            {
                ultimaPosicaoSegura = transform.position;
            }
        }
        else
        {
            tempoNoChao = 0f;
        }
    }

    private void FixedUpdate()
    {
        // SE TOMOU KNOCKBACK, PARA TUDO E IGNORA OS COMANDOS DE ANDAR!
        if (tomouKnockback) return;

        // Se não tomou, anda normalmente:
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
