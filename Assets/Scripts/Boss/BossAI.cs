using UnityEngine;

public class BossAI : MonoBehaviour
{
    // =====================================================================
    // CONFIGURAÇÕES INSPECTOR
    // =====================================================================
    [Header("Movimento")]
    [SerializeField] private float speed = 2f;

    [Header("Ranges de Combate")]
    [SerializeField] private float attackRange = 2f;  // Corpo a corpo
    [SerializeField] private float spellRange = 8f;   // Magia (distância média)

    [Header("Cooldowns")]
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float spellCooldown = 4f;

    // =====================================================================
    // ESTADO INTERNO
    // =====================================================================

    private enum BossState
    {
        Idle,       // Parado
        Walking,    // Perseguindo o player
        Attacking,  // Ataque corpo-a-corpo
        Casting,    // Magia
        Hurt        // Tomando dano (NOVO ESTADO)
    }

    private BossState currentState = BossState.Idle;

    private float nextMeleeTime = 0f;
    private float nextSpellTime = 0f;

    // =====================================================================
    // REFERÊNCIAS E COMBATE
    // =====================================================================
    [Header("Spell")]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float spellOffsetY = 3f;

    [Header("Ataque Melee")]
    public Transform attackPoint;       // Objeto vazio na ponta da espada
    public float attackHitboxRadius = 1.5f; // Tamanho do corte
    public int meleeDamage = 2;         // Dano da espadada

    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;

    // =====================================================================
    // UNITY LIFECYCLE
    // =====================================================================

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        AtualizarDirecaoVisual();

        float distX = Mathf.Abs(transform.position.x - player.position.x);

        AtualizarEstado(distX);
    }

    void FixedUpdate()
    {
        if (currentState == BossState.Walking)
        {
            float direcaoX = player.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(direcaoX * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    // =====================================================================
    // MÁQUINA DE ESTADOS
    // =====================================================================

    void AtualizarEstado(float distX)
    {
        // ADICIONAMOS O HURT AQUI: Se ele estiver apanhando, batendo ou conjurando, não interrompe!
        if (currentState == BossState.Attacking || currentState == BossState.Casting || currentState == BossState.Hurt)
            return;

        bool meleeReady = Time.time >= nextMeleeTime;
        bool spellReady = Time.time >= nextSpellTime;

        if (distX <= attackRange && meleeReady)
        {
            EntrarEstado(BossState.Attacking);
        }
        else if (distX > attackRange && distX <= spellRange && spellReady)
        {
            EntrarEstado(BossState.Casting);
        }
        else if (distX > attackRange)
        {
            EntrarEstado(BossState.Walking);
        }
        else
        {
            EntrarEstado(BossState.Idle);
        }
    }

    void EntrarEstado(BossState novoEstado)
    {
        if (currentState == novoEstado) return;

        currentState = novoEstado;

        switch (novoEstado)
        {
            case BossState.Idle:
                anim.SetBool("Walk", false);
                break;

            case BossState.Walking:
                anim.SetBool("Walk", true);
                break;

            case BossState.Attacking:
                anim.SetBool("Walk", false);
                anim.SetTrigger("Attack");
                nextMeleeTime = Time.time + attackCooldown;
                break;

            case BossState.Casting:
                anim.SetBool("Walk", false);
                anim.SetTrigger("Cast");
                nextSpellTime = Time.time + spellCooldown;
                break;

            // ADICIONAMOS O COMPORTAMENTO DO HURT AQUI!
            case BossState.Hurt:
                anim.SetBool("Walk", false); // Para de andar
                anim.SetTrigger("Hurt");     // Toca a animação de dor
                break;
        }
    }

    // =====================================================================
    // ANIMATION EVENTS E RECEBER DANO
    // =====================================================================

    // O INIMIGO HEALTH CHAMA ESSA FUNÇÃO AQUI:
    public void ReceberDano()
    {
        anim.ResetTrigger("Hurt"); // <- ISSO limpa o "acúmulo" de hits na memória da Unity
        EntrarEstado(BossState.Hurt);
        
        // Libera o Boss automaticamente após 0.4 segundos (tempo ajustado para ver a dor!)
        Invoke(nameof(SairDoHurt), 0.4f); 
    }

    private void SairDoHurt()
    {
        if (currentState == BossState.Hurt)
        {
            anim.ResetTrigger("Hurt"); // Garante que não vai tocar denovo do nada
            currentState = BossState.Idle;
        }
    }

    public void OnActionAnimationEnd()
    {
        currentState = BossState.Idle;
    }

    public void LancarSpell()
    {
        if (spellPrefab == null || player == null) return;

        Vector3 posicao = player.position + new Vector3(0f, spellOffsetY, 0f);
        Instantiate(spellPrefab, posicao, Quaternion.identity);
    }

    // Essa função deve ser chamada pela janela Animation no frame exato do corte da espada!
    public void DarDanoMelee()
    {
        if (attackPoint == null) return;

        // Cria um círculo invisível na ponta da espada e procura quem ele acertou
        Collider2D[] acertados = Physics2D.OverlapCircleAll(attackPoint.position, attackHitboxRadius);

        foreach (Collider2D hit in acertados)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.tomaDano(meleeDamage, transform);
                }
            }
        }
    }

    // =====================================================================
    // VIRAR O SPRITE
    // =====================================================================

    void AtualizarDirecaoVisual()
    {
        float diferencaX = player.position.x - transform.position.x;

        if (Mathf.Abs(diferencaX) > 0.1f)
        {
            transform.localScale = diferencaX > 0
                ? new Vector3(-1f, 1f, 1f)
                : new Vector3(1f, 1f, 1f);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spellRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackPoint.position, attackHitboxRadius);
        }
    }
#endif

    // =====================================================================
    // CAUSAR DANO DE CONTATO
    // =====================================================================
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Dá 1 de dano e manda a posição do boss para o player calcular o knockback
                playerHealth.tomaDano(1, transform);
            }
        }
    }
}