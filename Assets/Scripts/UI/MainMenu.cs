using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // O nome da cena principal do seu jogo (por padrão costuma ser SampleScene, mas ajuste se for outro)
    public string nomeDaCenaDoJogo = "SampleScene";

    public void Jogar()
    {
        // Reseta o tempo (garantia caso tenha vindo de um Pause)
        Time.timeScale = 1f;
        
        // Carrega a cena do jogo
        SceneManager.LoadScene(nomeDaCenaDoJogo);
    }

    public void Sair()
    {
        Debug.Log("Saindo do Jogo...");
        
        // No Editor da Unity o Quit não faz nada visualmente, mas no jogo buildado ele fecha a janela.
        Application.Quit();
    }
}
