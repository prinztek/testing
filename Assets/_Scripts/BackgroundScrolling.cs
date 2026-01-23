using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    private float startPos;
    private float length;
    public GameObject cam;
    public float parallaxEffect;
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    public float smoothTime = 0.2f;
    private Vector3 velocity = Vector3.zero;

    // void LateUpdate()
    // {
    //     float distance = cam.transform.position.x * parallaxEffect;
    //     float movement = cam.transform.position.x * (1 - parallaxEffect);

    //     Vector3 targetPos = new Vector3(
    //         startPos + distance,
    //         transform.position.y,
    //         transform.position.z
    //     );

    //     transform.position = Vector3.SmoothDamp(
    //         transform.position,
    //         targetPos,
    //         ref velocity,
    //         smoothTime
    //     );

    //     if (movement > startPos + length)
    //         startPos += length;
    //     else if (movement < startPos - length)
    //         startPos -= length;
    // }

    void LateUpdate()
    {
        float camX = cam.transform.position.x;
        float distance = camX * parallaxEffect;
        float movement = camX * (1 - parallaxEffect);
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }
    }
}
