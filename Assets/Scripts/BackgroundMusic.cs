using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Garante que a música vai ficar repetindo (em loop)
        audioSource.loop = true;
        
        // Garante que a música comece a tocar assim que o jogo iniciar
        audioSource.playOnAwake = true;
    }

    void Start()
    {
        // Se a música não estiver tocando por algum motivo, forçamos o play
        if (!audioSource.isPlaying && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
