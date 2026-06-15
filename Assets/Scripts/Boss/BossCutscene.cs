using System.Collections;
using UnityEngine;

public class BossCutscene : MonoBehaviour
{
    [Header("Referências")]
    public GameObject bossObject;         // O GameObject do Boss que já está na cena (deixe desativado)
    public AudioSource musicaDaFase;      // Arraste o AudioSource que toca a música da fase atual
    public AudioClip musicaDoBoss;        // O arquivo de áudio (.mp3 ou .wav) da música do Boss

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

        // 2. Faz o Boss aparecer na tela
        if (bossObject != null)
            bossObject.SetActive(true);

        // 3. Troca a música (se o AudioSource da fase estiver configurado)
        if (musicaDaFase != null && musicaDoBoss != null)
        {
            musicaDaFase.Stop();
            musicaDaFase.clip = musicaDoBoss;
            musicaDaFase.Play();
        }

        // 4. Aguarda o tempo da "cutscene" para dar impacto
        yield return new WaitForSeconds(tempoDePausa);

        // 5. Devolve o controle ao jogador e a luta começa!
        if (movimentoPlayer != null)
            movimentoPlayer.enabled = true;

        // Desativa ou destrói o trigger para não rodar de novo
        gameObject.SetActive(false);
    }
}
