using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private PlayerAnimations animations;
    private PlayerControl control;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int damage = 5;
    [SerializeField] private float forcaKnockback = 5f;
    private Rigidbody2D rb;

    private void Awake()
    {
        animations = GetComponent<PlayerAnimations>();

        control = new PlayerControl();

        control.Player.Attack.performed += OnAttackPressed;
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        control.Enable();
    }

    private void OnDisable()
    {
        control.Disable();
    }

    private void OnAttackPressed(InputAction.CallbackContext context)
    {
        Debug.Log("ATAQUE!");
        animations.TriggerAttack();
    }

    public void Attack()
    {
        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRange,
                enemyLayer
            );

        foreach (Collider2D enemy in enemies)
        {
            Debug.Log("Acertou: " + enemy.name);

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRange
        );
    }
}
