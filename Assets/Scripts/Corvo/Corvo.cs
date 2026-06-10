using UnityEngine;

public class Corvo : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float detectionRange = 6f;
    private Animator animator;
    private Transform player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            animator.SetBool("IsFly", true);

            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );

            if (player.position.x > transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            animator.SetBool("IsFly", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Colidiu com: " + col.gameObject.name);
        if (col.gameObject.CompareTag("Player")){
            Debug.Log("Acertou o player!");
            PlayerHealth health = col.gameObject.GetComponent<PlayerHealth>();
            Debug.Log("Health encontrada? " + (health != null));
            if (health != null)
            {
                Debug.Log("Chamando tomaDano");
                health.tomaDano(20);
            }
        }
    }
}