using UnityEngine;

public class SpellInvocada : MonoBehaviour
{
    // =====================================================================
    // CONFIGURAÇÕES INSPECTOR
    // =====================================================================
    [Header("Combate")]
    [SerializeField] private float dano = 20f;

    [Header("Movimento")]
    [SerializeField] private float velocidade = 3f;
    [SerializeField] private float velocidadeMaxima = 8f; // Acelera conforme desce
    [SerializeField] private float aceleracao = 2f;

    [Header("Tempo de Vida")]
    [SerializeField] private float tempoMaximo = 4f; // Se não acertar nada, some após esse tempo

    // =====================================================================
    // ESTADO INTERNO
    // =====================================================================
    private float velocidadeAtual;
    private float tempoVivo = 0f;
    private bool atingiuAlgo = false;

    private Animator anim;

    // =====================================================================
    // UNITY LIFECYCLE
    // =====================================================================

    void Awake()
    {
        anim = GetComponent<Animator>();
        velocidadeAtual = velocidade;
    }

    void Update()
    {
        if (atingiuAlgo) return;

        // Controla o tempo de vida
        tempoVivo += Time.deltaTime;
        if (tempoVivo >= tempoMaximo)
        {
            Destruir();
            return;
        }

        // Acelera progressivamente enquanto desce
        velocidadeAtual = Mathf.MoveTowards(
            velocidadeAtual,
            velocidadeMaxima,
            aceleracao * Time.deltaTime
        );

        // Move para baixo
        transform.Translate(Vector2.down * velocidadeAtual * Time.deltaTime);
    }

    // =====================================================================
    // DETECÇÃO DE COLISÃO
    // =====================================================================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (atingiuAlgo) return;

        if (other.CompareTag("Player"))
        {
            atingiuAlgo = true;

            // Aplica dano ao player
            // Descomente a linha abaixo quando tiver o script de HP do player:
            // other.GetComponent<PlayerHealth>()?.ReceberDano(dano);

            Destruir();
        }

        // Colide com o chão ou plataformas (opcional)
        if (other.CompareTag("Ground"))
        {
            atingiuAlgo = true;
            Destruir();
        }
    }

    // =====================================================================
    // DESTRUIÇÃO
    // =====================================================================

    /// <summary>
    /// Toca a animação de impacto (se existir) e destrói o objeto.
    /// Se não tiver animação de impacto, apenas destrói.
    /// </summary>
    void Destruir()
    {
        // Para o movimento
        velocidadeAtual = 0f;

        // Se tiver animação de impacto no Animator, dispara ela
        if (anim != null)
        {
            anim.SetTrigger("Hit");
            // Destrói após o tempo da animação de impacto (ajuste conforme a duração)
            Destroy(gameObject, 0.4f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =====================================================================
    // ANIMATION EVENT
    // =====================================================================

    /// <summary>
    /// Chame via Animation Event no último frame da animação de Hit,
    /// caso prefira que a animação controle o momento exato da destruição.
    /// </summary>
    public void OnHitAnimationEnd()
    {
        Destroy(gameObject);
    }

    // =====================================================================
    // GIZMOS
    // =====================================================================

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Mostra a direção do movimento no Scene View
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.5f);
        Gizmos.DrawWireSphere(transform.position + Vector3.down * 1.5f, 0.1f);
    }
#endif
}