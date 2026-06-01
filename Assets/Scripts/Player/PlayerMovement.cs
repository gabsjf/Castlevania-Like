using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;
    private PlayerControl control;
    private Vector2 movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        control = new PlayerControl();
    }

    private void OnEnable()
    {
        control.Enable();
    }

    private void OnDisable()
    {
        control.Disable();
    }


    void Update()
    {
        movement = control.Player.Move.ReadValue<Vector2>();
        Debug.Log(movement);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            movement.x * speed,
            rb.linearVelocity.y
        );
    }
}
