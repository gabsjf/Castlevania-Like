using UnityEngine;

public class BossArenaTrigger : MonoBehaviour
{
    [SerializeField] private BackgroundController[] backgrounds;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            foreach (BackgroundController bg in backgrounds)
            {
                bg.enabled = false;
            }
        }
    }
}