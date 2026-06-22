using System.Collections;
using UnityEngine;

public class MusicFadeTrigger : MonoBehaviour
{
    [Header("Referência")]
    public AudioSource musicaDaFase; // Arraste o objeto que tem a música da fase aqui

    [Header("Configurações")]
    public float tempoDeFade = 2f; // Em segundos, quanto tempo leva pra música zerar

    private bool jaAtivado = false;
    private float volumeInicial;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!jaAtivado && collision.CompareTag("Player"))
        {
            jaAtivado = true;
            if (musicaDaFase != null)
            {
                volumeInicial = musicaDaFase.volume;
                StartCoroutine(FadeOutCoroutine());
            }
        }
    }

    private IEnumerator FadeOutCoroutine()
    {
        float timer = 0f;

        while (timer < tempoDeFade)
        {
            timer += Time.deltaTime;
            // Abaixa o volume gradativamente de 'volumeInicial' até 0
            musicaDaFase.volume = Mathf.Lerp(volumeInicial, 0f, timer / tempoDeFade);
            yield return null; // Espera o próximo frame
        }

        musicaDaFase.volume = 0f;
        musicaDaFase.Stop();
        
        // Restaura o volume original para que, quando você entrar na Cutscene do Boss
        // e o áudio for trocado, a nova música não toque muda!
        musicaDaFase.volume = volumeInicial;
    }
}
