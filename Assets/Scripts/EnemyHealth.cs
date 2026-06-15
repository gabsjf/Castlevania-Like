using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;

    [Header("Som")]
    [SerializeField] private AudioClip somDeMorte;
    [Range(0f, 1f)]
    [SerializeField] private float volumeMorte = 1f;

    [Header("Animações")]
    [SerializeField] private string triggerMorte = "Dead";

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

        if (somDeMorte != null)
        {
            // Cria um player de som 2D temporário para evitar a atenuação 3D da Unity
            GameObject tempAudio = new GameObject("TempAudio_DeathSFX");
            AudioSource source = tempAudio.AddComponent<AudioSource>();
            source.clip = somDeMorte;
            source.volume = volumeMorte;
            source.spatialBlend = 0f; // 2D (sem perda de volume pela distância da câmera no eixo Z)
            source.Play();
            Destroy(tempAudio, somDeMorte.length);
        }

        animator.SetTrigger(triggerMorte);

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