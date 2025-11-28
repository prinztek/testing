using UnityEngine;

public class MovingPlatformRehopeGames : MonoBehaviour
{
    public Transform posA;
    public Transform posB;
    public float speed = 2f;

    private Vector3 targetPos;
    private Vector3 moveDirection;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        targetPos = posB.position;
        CalculateDirection();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, posA.position) < 0.05f)
        {
            targetPos = posB.position;
            CalculateDirection();
        }
        else if (Vector2.Distance(transform.position, posB.position) < 0.05f)
        {
            targetPos = posA.position;
            CalculateDirection();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }

    void CalculateDirection()
    {
        moveDirection = (targetPos - transform.position).normalized;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Move move = collision.GetComponent<Move>();
            if (move != null)
                move.SetPlatform(rb);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Move move = collision.GetComponent<Move>();
            if (move != null)
                move.ClearPlatform(rb);
        }
    }
}
