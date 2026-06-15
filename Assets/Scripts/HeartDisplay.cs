using UnityEngine;
using UnityEngine.UI;

public class HeartDisplay : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite heartFull;
    public Sprite heartHalf;
    public Sprite heartEmpty;

    [Header("Referências")]
    public PlayerHealth playerHealth;
    public Transform heartsContainer; // o pai dos slots no Canvas
    public GameObject heartPrefab;    // prefab com um Image component

    private Image[] heartSlots;

    void Start()
    {
        // Cria os slots dinamicamente baseado no maxHearts
        heartSlots = new Image[playerHealth.CoracoesMax];
        for (int i = 0; i < playerHealth.CoracoesMax; i++)
        {
            GameObject slot = Instantiate(heartPrefab, heartsContainer);
            heartSlots[i] = slot.GetComponent<Image>();
        }

        playerHealth.OnHealthChanged += UpdateHearts;
        UpdateHearts(); // estado inicial
    }

    void UpdateHearts()
    {
        for (int i = 0; i < heartSlots.Length; i++)
        {
            float hpForThisSlot = Mathf.Clamp(playerHealth.vidaAtual - (i * 2f), 0f, 2f);

            if (hpForThisSlot >= 2f)
                heartSlots[i].sprite = heartFull;
            else if (hpForThisSlot >= 1f)
                heartSlots[i].sprite = heartHalf;
            else
                heartSlots[i].sprite = heartEmpty;
        }
    }

    void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateHearts;
    }
}
