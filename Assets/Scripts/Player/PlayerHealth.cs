using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Número de corações (cada coração = 2 de HP internamente, para suportar meios corações)
    [SerializeField] private int coracoesMax = 6;

    private PlayerAnimations animations;
    [SerializeField] private float forcaKnockback = 10f;
    [SerializeField] private float tempoKnockback = 0.3f;
    [SerializeField] private float tempoInvencivel = 1f;

    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private PlayerMovement movimento;

    // Vida agora é float para suportar meios corações (1 = meio, 2 = cheio)
    public float vidaAtual { get; private set; }
    public int CoracoesMax => coracoesMax;

    // A HUD vai escutar esse evento para se atualizar
    public event Action OnHealthChanged;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animations = GetComponent<PlayerAnimations>();
        movimento = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    void Start()
    {
        vidaAtual = coracoesMax * 2f; // começa com todos os corações cheios
    }

    public void tomaDano(int dano, Transform posicaoInimigo)
    {
        if (isInvincible) return;

        animations.TriggerHurt();

        vidaAtual = Mathf.Max(0, vidaAtual - dano);
        OnHealthChanged?.Invoke(); // avisa a HUD

        Debug.Log("Vida atual: " + vidaAtual);

        if (posicaoInimigo != null)
        {
            float direcaoDoEmpurrao = Mathf.Sign(transform.position.x - posicaoInimigo.position.x);
            rb.linearVelocity = new Vector2(direcaoDoEmpurrao * forcaKnockback, forcaKnockback * 0.5f);
            StartCoroutine(RotinaKnockback());
        }

        if (vidaAtual <= 0)
            Morrer();
        else
            StartCoroutine(RotinaInvencibilidade());
    }

    // Opcional: curar o jogador (útil para poções, etc.)
    public void Curar(float quantidade)
    {
        vidaAtual = Mathf.Min(coracoesMax * 2f, vidaAtual + quantidade);
        OnHealthChanged?.Invoke();
    }

    private IEnumerator RotinaKnockback()
    {
        if (movimento != null) movimento.tomouKnockback = true;
        yield return new WaitForSeconds(tempoKnockback);
        if (movimento != null) movimento.tomouKnockback = false;
    }

    private IEnumerator RotinaInvencibilidade()
    {
        isInvincible = true;
        float tempoPiscando = 0;
        while (tempoPiscando < tempoInvencivel)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.1f);
            tempoPiscando += 0.2f;
        }
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        isInvincible = false;
    }

    void Morrer()
    {
        Destroy(gameObject);
    }
}