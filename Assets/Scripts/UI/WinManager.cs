using UnityEngine;
using System.Collections;

public class WinManager : MonoBehaviour
{
    [Header("Configurações")]
    public GameObject painelVitoria; // Arraste o seu Painel de UI aqui
    public AudioSource musicaDoBoss; // Arraste a música que está tocando
    public float tempoParaAparecer = 3f; // Segundos esperando o boss cair

    public void DeclararVitoria()
    {
        // 1. Para a música instantaneamente para dar impacto
        if (musicaDoBoss != null)
        {
            musicaDoBoss.Stop();
        }

        // 2. Inicia a contagem para mostrar a tela
        StartCoroutine(RotinaDeVitoria());
    }

    private IEnumerator RotinaDeVitoria()
    {
        // Espera o tempo configurado (assistindo a animação de morte)
        yield return new WaitForSeconds(tempoParaAparecer);

        // Ativa a tela de "Obrigado por Jogar"
        if (painelVitoria != null)
        {
            painelVitoria.SetActive(true);
        }

        // Pausa o jogo inteiro (zera o tempo) para o player não ficar andando no fundo
        Time.timeScale = 0f;
    }
}
