using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float length, startPosX, startPosY;
    public GameObject cam;

    public float parallaxEffectX;
    public float parallaxEffectY;

    void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y; 

        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        float tempX = (cam.transform.position.x * (1 - parallaxEffectX));
        float distanceX = (cam.transform.position.x * parallaxEffectX);

        float distanceY = (cam.transform.position.y * parallaxEffectY);

        transform.position = new Vector3(startPosX + distanceX, startPosY + distanceY, transform.position.z);

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