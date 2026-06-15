using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool JogoPausado = false;

    [Header("Referência da Interface")]
    public GameObject painelDePause;
    
    [Header("Nome da Cena do Menu")]
    public string nomeDaCenaMenu = "Menu";

    void Start()
    {
        // Garante que o painel comece escondido e o tempo rodando
        painelDePause.SetActive(false);
        Time.timeScale = 1f;
        JogoPausado = false;
    }

    void Update()
    {
        // Verifica se há teclado conectado e se o ESC foi pressionado neste frame
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (JogoPausado)
            {
                Retomar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        painelDePause.SetActive(true);
        Time.timeScale = 0f; // Congela o tempo (física, Update, animações)
        JogoPausado = true;
    }

    public void Retomar()
    {
        painelDePause.SetActive(false);
        Time.timeScale = 1f; // Descongela o tempo
        JogoPausado = false;
    }

    public void VoltarAoMenu()
    {
        // MUITO IMPORTANTE: Descongelar o tempo antes de sair, senão o menu ficará travado!
        Time.timeScale = 1f;
        JogoPausado = false;
        
        SceneManager.LoadScene(nomeDaCenaMenu);
    }
}
