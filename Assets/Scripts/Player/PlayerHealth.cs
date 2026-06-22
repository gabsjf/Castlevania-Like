using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // NÃºmero de coraÃ§Ãµes (cada coraÃ§Ã£o = 2 de HP internamente, para suportar meios coraÃ§Ãµes)
    [SerializeField] private int coracoesMax = 6;

    [Header("Audio")]
    [SerializeField] private AudioClip somDeDano;
    private AudioSource audioSource;

    private PlayerAnimations animations;
    [SerializeField] private float forcaKnockback = 10f;
    [SerializeField] private float tempoKnockback = 0.3f;
    [SerializeField] private float tempoInvencivel = 1f;

    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private PlayerMovement movimento;

    // Vida agora Ã© float para suportar meios coraÃ§Ãµes (1 = meio, 2 = cheio)
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
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        
        vidaAtual = coracoesMax * 2f; // Define a vida antes do HeartDisplay ler no Start
    }

    void Start()
    {
        OnHealthChanged?.Invoke(); // Garante que a HUD seja atualizada se precisar
    }

    public void tomaDano(int dano, Transform posicaoInimigo)
    {
        if (isInvincible) return;

        animations.TriggerHurt();

        if (somDeDano != null && audioSource != null)
        {
            audioSource.PlayOneShot(somDeDano);
        }

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

    // Opcional: curar o jogador (Ãºtil para poÃ§Ãµes, etc.)
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

    // Chamado pelo VoidZone quando o player cai no buraco
    public void TomaDanoDeQueda(Vector3 posicaoSegura)
    {
        if (vidaAtual <= 0) return; // Se já morreu caindo, ignora

        // Dano de meio coração (1 de HP interno)
        vidaAtual = Mathf.Max(0, vidaAtual - 1);
        OnHealthChanged?.Invoke(); 
        
        if (somDeDano != null && audioSource != null)
        {
            audioSource.PlayOneShot(somDeDano);
        }

        if (vidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            // Teletransporta de volta pra plataforma segura e reseta a velocidade de queda
            transform.position = posicaoSegura;
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(RotinaInvencibilidade());
        }
    }

    void Morrer()
    {
        // Desativa a colisão para ele não ser empurrado por inimigos enquanto morre
        GetComponent<Collider2D>().enabled = false;
        
        // Zera a gravidade e o movimento para ele ficar paradinho (opcional, ou pode deixar ele cair)
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        // Desliga o controle de movimento para o jogador não andar mais
        if (movimento != null) movimento.enabled = false;

        // Avisa o Manager para escurecer a tela e reiniciar o jogo
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.IniciarGameOver();
        }
        else
        {
            // Fallback caso esqueça de colocar o Manager na cena
            Destroy(gameObject);
        }
    }
}
