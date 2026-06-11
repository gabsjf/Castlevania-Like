using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;

    private Animator animator;
    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        // Se já morreu, ignora o dano
        if (isDead) return;

        health -= damage;

        Debug.Log("Vida restante: " + health);

        // --- COMUNICAÇÃO COM O BOSS ---
        // Procura se esse inimigo tem o cérebro de Boss. Se tiver, avisa que tomou hit!
        BossAI bossAI = GetComponent<BossAI>();
        if (bossAI != null)
        {
            bossAI.ReceberDano();
        }
        // ------------------------------

        if (health <= 0)
        {
            Die();
        }
        else if (bossAI == null)
        {
            // Opcional: Se você tiver animação de Hurt para os inimigos comuns,
            // pode tocar o gatilho "Hurt" do Animator deles aqui!
            // animator.SetTrigger("Hurt");
        }
    }

    public void Die()
    {
        isDead = true; // Garante que a trava lá de cima vai funcionar!

        animator.SetTrigger("Dead");

        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        GetComponent<Collider2D>().enabled = false;

        GetComponent<Rigidbody2D>().gravityScale = 0;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}