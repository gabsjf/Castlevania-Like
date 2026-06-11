using UnityEngine;

public class Ghoul : MonoBehaviour
{
    [SerializeField] private float speed = 2f;

    private Transform player;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("Player encontrado!");
        }
        else
        {
            Debug.Log("Player NÃO encontrado!");
        }
    }

    private void Update()
    {

        Debug.Log("Perseguindo...");
        if (player == null) return;

        Vector2 targetPosition = new Vector2(
            player.position.x,
            transform.position.y
        );

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = col.gameObject.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.tomaDano(10);
            }
        }
    }
}