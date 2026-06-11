using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int vidaMax = 50;
    private PlayerAnimations animations;
    [SerializeField] private float forcaKnockback = 10f;
    [SerializeField] private float tempoKnockback = 0.3f;

    // Nossas novas variáveis para a invencibilidade
    [SerializeField] private float tempoInvencivel = 1f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer; // Para fazer o personagem piscar

    private Rigidbody2D rb;
    private int vidaAtual;
    private PlayerMovement movimento;

    void Start()
    {
        vidaAtual = vidaMax;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animations = GetComponent<PlayerAnimations>();
        movimento = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Pega o desenho do player
    }

    public void tomaDano(int dano, Transform posicaoInimigo)
    {
        // 1. SE ESTIVER INVENCÍVEL, IGNORA O DANO E SAI DA FUNÇÃO NA HORA!
        if (isInvincible) return;

        animations.TriggerHurt();

        vidaAtual -= dano;
        Debug.Log("Vida atual: " + vidaAtual);

        if (posicaoInimigo != null)
        {
            float direcaoDoEmpurrao = transform.position.x - posicaoInimigo.position.x;
            direcaoDoEmpurrao = Mathf.Sign(direcaoDoEmpurrao);

            rb.linearVelocity = new Vector2(direcaoDoEmpurrao * forcaKnockback, forcaKnockback * 0.5f);

            StartCoroutine(RotinaKnockback());
        }

        if (vidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            // 2. SE NÃO MORREU, FICA INVENCÍVEL POR 1 SEGUNDO
            StartCoroutine(RotinaInvencibilidade());
        }
    }

    private IEnumerator RotinaKnockback()
    {
        if (movimento != null)
        {
            movimento.tomouKnockback = true;
        }

        yield return new WaitForSeconds(tempoKnockback);

        if (movimento != null)
        {
            movimento.tomouKnockback = false;
        }
    }

    // 3. A NOSSA NOVA ROTINA DE INVENCIBILIDADE E PISCAR
    private IEnumerator RotinaInvencibilidade()
    {
        isInvincible = true; // Liga a trava de invencibilidade

        // Faz o personagem piscar repetindo esse ciclo
        float tempoPiscando = 0;
        while (tempoPiscando < tempoInvencivel)
        {
            // Deixa meio transparente
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            yield return new WaitForSeconds(0.1f);

            // Volta a cor ao normal
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.1f);

            tempoPiscando += 0.2f; // Conta o tempo que já passou (0.1 + 0.1)
        }

        // Garante que a cor vai estar normal no final
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);

        isInvincible = false; // Desliga a trava e permite tomar dano de novo
    }

    void Morrer()
    {
        Destroy(gameObject);
    }
}