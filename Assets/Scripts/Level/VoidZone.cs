using UnityEngine;

public class VoidZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se quem caiu no buraco foi o Player
        if (collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            PlayerMovement mov = collision.GetComponent<PlayerMovement>();
            
            if (health != null && mov != null)
            {
                // Dá o dano e manda ele de volta pro último lugar seguro
                health.TomaDanoDeQueda(mov.ultimaPosicaoSegura);
            }
        }
    }
}
