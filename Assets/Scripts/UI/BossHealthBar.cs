using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("Configurações da UI")]
    public Slider healthSlider;

    // Essa função será chamada automaticamente pelo BossCutscene quando a luta começar
    public void Setup(EnemyHealth bossHealth)
    {
        if (bossHealth == null) return;

        // "Inscreve" esta barra no evento de dano do Boss
        bossHealth.OnHealthChanged += UpdateHealthBar;
        
        // Garante que o Slider comece cheio (valor entre 0 e 1)
        if (healthSlider != null)
        {
            healthSlider.maxValue = 1f;
            healthSlider.value = 1f; 
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            // Transforma a vida em uma porcentagem (ex: 50 de 100 = 0.5)
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }
}
