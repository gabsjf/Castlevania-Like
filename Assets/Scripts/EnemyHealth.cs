using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 3; // Funciona como Max Health
    private int currentHealth;

    // Evento disparado quando o inimigo toma dano (passa a vida atual e a vida máxima)
    public event System.Action<int, int> OnHealthChanged;

    [Header("Som")]
    [SerializeField] private AudioClip somDeMorte;
    [Range(0f, 1f)]
    [SerializeField] private float volumeMorte = 1f;

    [Header("Animações")]
    [SerializeField] private string triggerMorte = "Dead";

    [Header("Fim de Jogo (Apenas para Bosses)")]
    public WinManager gerenciadorVitoria; // Deixe vazio para inimigos comuns

    private Animator animator;
    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = health; // Inicializa a vida atual com o valor máximo
    }

    public void TakeDamage(int damage)
    {
        // Se já morreu, ignora o dano
        if (isDead) return;

        currentHealth -= damage;
        
        // Avisa quem estiver escutando (ex: a UI) que a vida mudou
        OnHealthChanged?.Invoke(currentHealth, health);

        // --- COMUNICAÇÃO COM O BOSS E MORTE ---
        BossAI bossAI = GetComponent<BossAI>();

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (bossAI != null)
        {
            // Só toca animação de dor se ele NÃO morreu.
            // Isso evita que a Unity tente tocar "Hurt" e "Dead" ao mesmo tempo e trave!
            bossAI.ReceberDano();
        }
        // ------------------------------
    }

    public void Die()
    {
        isDead = true; // Garante que a trava lá de cima vai funcionar!

        // Se esse inimigo for o Boss final (tiver o gerenciador linkado), aciona o fim de jogo!
        if (gerenciadorVitoria != null)
        {
            gerenciadorVitoria.DeclararVitoria();
        }

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