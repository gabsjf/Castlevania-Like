using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float length;
    private float startPosX;
    private float startPosY;

    public GameObject cam;

    public float parallaxEffectX;
    public float parallaxEffectY;

    [SerializeField] private float stopAtX = 100f;

    void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;

        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        // Para o parallax quando chegar na arena do boss
        if (cam.transform.position.x >= stopAtX)
        {
            enabled = false;
            return;
        }

        float tempX = cam.transform.position.x * (1 - parallaxEffectX);
        float distanceX = cam.transform.position.x * parallaxEffectX;

        float distanceY = cam.transform.position.y * parallaxEffectY;

        transform.position = new Vector3(
            startPosX + distanceX,
            startPosY + distanceY,
            transform.position.z
        );

        if (tempX > startPosX + length)
        {
            startPosX += length;
        }
        else if (tempX < startPosX - length)
        {
            startPosX -= length;
        }
    }
}