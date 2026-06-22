using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameOverManager : MonoBehaviour
{
    // O Singleton permite que o Player ache esse script facilmente de qualquer lugar
    public static GameOverManager Instance;
    [Header("Configurações")]
    public Image telaPreta; // Arraste a imagem preta aqui
    public float tempoDeFade = 2f; // Segundos que demora para escurecer
    public float tempoEsperandoMorto = 1.5f; // Segundos antes de começar a escurecer
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Garante que a tela preta comece transparente e desligada (pra não atrapalhar no editor)
        if (telaPreta != null)
        {
            Color c = telaPreta.color;
            c.a = 0f;
            telaPreta.color = c;
            telaPreta.gameObject.SetActive(false);
        }
    }
    public void IniciarGameOver()
    {
        StartCoroutine(RotinaGameOver());
    }
    private IEnumerator RotinaGameOver()
    {
        // 1. Espera um tempinho pro jogador sofrer vendo o personagem cair/morrer
        yield return new WaitForSeconds(tempoEsperandoMorto);
        // 2. Vai deixando a tela preta aos poucos
        if (telaPreta != null)
        {
            telaPreta.gameObject.SetActive(true);
            float tempoPassado = 0f;
            Color c = telaPreta.color;
            while (tempoPassado < tempoDeFade)
            {
                tempoPassado += Time.deltaTime;
                c.a = Mathf.Clamp01(tempoPassado / tempoDeFade);
                telaPreta.color = c;
                yield return null;
            }
        }
        // 3. Quando ficar 100% preta, recarrega a cena (do zero)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
