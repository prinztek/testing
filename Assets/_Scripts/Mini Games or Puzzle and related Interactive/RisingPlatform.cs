using UnityEngine;

public class RisingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 targetPosition;
    public float speed = 2f;

    private Vector3 startPosition;
    private bool moveUp;

    void Start()
    {
        startPosition = transform.position;
    }
    void Update()
    {
        if (moveUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
        }
    }

    public void SetRaised(bool raised)
    {
        moveUp = raised;
    }
}
