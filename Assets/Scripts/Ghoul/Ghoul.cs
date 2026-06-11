using UnityEngine;

public class Ghoul : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float detectionRange = 6f;

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
        if (player == null) return;

        // 2. Calculamos a distância entre o Ghoul e o Player
        float distance = Vector2.Distance(transform.position, player.position);

        // 3. O Ghoul só persegue se a distância for menor ou igual ao detectionRange
        if (distance <= detectionRange)
        {
            Debug.Log("Perseguindo...");

            Vector2 targetPosition = new Vector2(
                player.position.x,
                transform.position.y
            );

            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            // Gira o Ghoul de acordo com a posição do player
            if (player.position.x > transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = col.gameObject.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.tomaDano(6,transform);
            }
        }
    }
}