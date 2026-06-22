using System.Collections;
using UnityEngine;

public class BossCutscene : MonoBehaviour
{
    [Header("Referências")]
    public GameObject bossObject;         // O GameObject do Boss que já está na cena (deixe desativado)
    public GameObject paredeInvisivel;    // Barreira que impede o player de fugir da arena
    public AudioSource musicaDaFase;      // Arraste o AudioSource que toca a música da fase atual
    public AudioClip musicaDoBoss;        // O arquivo de áudio (.mp3 ou .wav) da música do Boss

    [Header("Cinemática Epic")]
    public CinematicBars barrasCinematicas;   // O script das barras pretas
    public GameObject cameraDoBoss;           // A Virtual Camera do Cinemachine focada no boss
    public string triggerAnimacaoBoss = "Roar"; // Nome do Trigger da animação de entrada do Boss

    [Header("UI do Boss")]
    public GameObject barraDeVidaUI;          // O GameObject da barra de vida do Boss (inicia desativado)

    [Header("Configurações")]
    public float tempoDePausa = 1.5f;     // Quanto tempo o jogo "para" quando o boss aparece

    private bool jaAtivado = false;

    void Start()
    {
        // Garante que o boss comece desativado (invisível e sem agir)
        if (bossObject != null)
            bossObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!jaAtivado && collision.CompareTag("Player"))
        {
            jaAtivado = true;
            StartCoroutine(RotinaDaCutscene(collision.gameObject));
        }
    }

    private IEnumerator RotinaDaCutscene(GameObject player)
    {
        // 1. Opcional: Trava o movimento do jogador temporariamente
        PlayerMovement movimentoPlayer = player.GetComponent<PlayerMovement>();
        if (movimentoPlayer != null)
        {
            // Você pode precisar adicionar um "public bool podeSeMover = true" no seu PlayerMovement 
            // ou apenas desativar o componente
            movimentoPlayer.enabled = false;
            
            // Para zerar a velocidade do player enquanto ele assiste o Boss aparecer
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // 2. Faz o Boss aparecer na tela e fecha a arena
        if (bossObject != null)
            bossObject.SetActive(true);
            
        if (paredeInvisivel != null)
            paredeInvisivel.SetActive(true);

        // --- INÍCIO DA CUTSCENE CINEMÁTICA ---
        // Desce as barras pretas
        if (barrasCinematicas != null)
            barrasCinematicas.ShowBars();

        // Ativa a Câmera do Boss (a Cinemachine move sozinha)
        if (cameraDoBoss != null)
            cameraDoBoss.SetActive(true);

        // Toca a animação de entrada do Boss
        if (bossObject != null && !string.IsNullOrEmpty(triggerAnimacaoBoss))
        {
            Animator bossAnim = bossObject.GetComponent<Animator>();
            if (bossAnim != null) bossAnim.SetTrigger(triggerAnimacaoBoss);
        }
        // ---------------------------------------

        // 3. Troca a música (se o AudioSource da fase estiver configurado)
        if (musicaDaFase != null && musicaDoBoss != null)
        {
            musicaDaFase.Stop();
            musicaDaFase.clip = musicaDoBoss;
            musicaDaFase.Play();
        }

        // 4. Aguarda o tempo da "cutscene" para dar impacto
        yield return new WaitForSeconds(tempoDePausa);

        // --- FIM DA CUTSCENE CINEMÁTICA ---
        // Sobe as barras
        if (barrasCinematicas != null)
            barrasCinematicas.HideBars();

        // Volta a câmera pro Player
        if (cameraDoBoss != null)
            cameraDoBoss.SetActive(false);
        // -----------------------------------

        // 5. Devolve o controle ao jogador e a luta começa!
        if (movimentoPlayer != null)
            movimentoPlayer.enabled = true;

        // --- ATIVA A BARRA DE VIDA ---
        if (barraDeVidaUI != null)
        {
            barraDeVidaUI.SetActive(true);
            BossHealthBar scriptBarra = barraDeVidaUI.GetComponent<BossHealthBar>();
            
            if (scriptBarra != null && bossObject != null)
            {
                EnemyHealth saudeDoBoss = bossObject.GetComponent<EnemyHealth>();
                scriptBarra.Setup(saudeDoBoss);
            }
        }

        // Desativa ou destrói o trigger para não rodar de novo
        gameObject.SetActive(false);
    }
}
