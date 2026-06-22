using System.Collections;
using UnityEngine;

public class CinematicBars : MonoBehaviour
{
    [Header("Barras Pretas")]
    public RectTransform topBar;
    public RectTransform bottomBar;
    
    [Header("Configurações")]
    public float animationTime = 1f;
    public float offscreenOffset = 1000f; // Distância gigante para garantir que sai da tela

    private float topVisibleY;
    private float bottomVisibleY;

    private void Awake()
    {
        if (topBar != null && bottomBar != null)
        {
            // Grava a posição exata que você deixou no Editor como a "Posição Visível"
            topVisibleY = topBar.anchoredPosition.y;
            bottomVisibleY = bottomBar.anchoredPosition.y;

            // Já começa o jogo escondendo elas (empurrando 1000 pixels para fora)
            topBar.anchoredPosition = new Vector2(topBar.anchoredPosition.x, topVisibleY + offscreenOffset);
            bottomBar.anchoredPosition = new Vector2(bottomBar.anchoredPosition.x, bottomVisibleY - offscreenOffset);
        }
    }

    public void ShowBars()
    {
        StopAllCoroutines();
        // Volta exatamente para a posição que você configurou no Editor
        StartCoroutine(AnimateBars(topVisibleY, bottomVisibleY));
    }

    public void HideBars()
    {
        StopAllCoroutines();
        // Empurra 1000 pixels para fora
        StartCoroutine(AnimateBars(topVisibleY + offscreenOffset, bottomVisibleY - offscreenOffset));
    }

    private IEnumerator AnimateBars(float targetTopY, float targetBottomY)
    {
        float timer = 0f;
        Vector2 startTop = topBar.anchoredPosition;
        Vector2 startBottom = bottomBar.anchoredPosition;

        Vector2 targetTop = new Vector2(startTop.x, targetTopY);
        Vector2 targetBottom = new Vector2(startBottom.x, targetBottomY);

        while (timer < animationTime)
        {
            timer += Time.deltaTime;
            
            // Calculo de suavização (SmoothStep) para a barra não bater seco no final
            float t = timer / animationTime;
            t = t * t * (3f - 2f * t); 

            topBar.anchoredPosition = Vector2.Lerp(startTop, targetTop, t);
            bottomBar.anchoredPosition = Vector2.Lerp(startBottom, targetBottom, t);
            
            yield return null;
        }

        topBar.anchoredPosition = targetTop;
        bottomBar.anchoredPosition = targetBottom;
    }
}
