using UnityEngine;

public class Ghoul : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float detectionRange = 6f;

    // cooldown para não dar dano múltiplas vezes seguidas
    [SerializeField] private float tempoCooldownDano = 1.2f;
    private float timerDano = 0f;

    private Transform player;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null) return;

        // decrementa o timer a cada frame
        if (timerDano > 0)
            timerDano -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (player.position.x > transform.position.x)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else
                transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Colisão! Timer: {timerDano} | Ghoul: {gameObject.name}");

            if (timerDano <= 0)
            {
                PlayerHealth health = col.gameObject.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    Debug.Log($"Dano aplicado por: {gameObject.name} | Vida antes: {health.vidaAtual}");
                    health.tomaDano(1, transform);
                    timerDano = tempoCooldownDano;
                }
            }
        }
    }
}