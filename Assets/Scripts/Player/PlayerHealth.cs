using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int vidaMax = 50;
    private int vidaAtual;
    void Start()
    {
        vidaAtual = vidaMax;

    }

     public void tomaDano(int dano)
    {
        vidaAtual -= dano;
        Debug.Log("Vida atual: " + vidaAtual);
        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        Debug.Log("Vida atual: " + vidaAtual);
    }
}
