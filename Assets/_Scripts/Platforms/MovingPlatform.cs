using UnityEngine;
public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Vector3 target;
    private Rigidbody2D _rb;
    private Vector3 _previousPosition;
    public Vector3 Velocity { get; private set; }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        target = pointB.position;
        _previousPosition = _rb.position;
    }

    // Rigidbody method for consistent physics updates
    private void FixedUpdate()
    {
        Vector2 currentPosition = _rb.position;
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, target, speed * Time.fixedDeltaTime);
        _rb.MovePosition(newPosition);

        // Calculate platform velocity manually
        Velocity = ((Vector3)newPosition - _previousPosition) / Time.fixedDeltaTime;
        _previousPosition = newPosition;

        // Switch target if reached
        if (Vector2.Distance(newPosition, target) < 0.1f)
        {
            target = target == pointA.position ? pointB.position : pointA.position;
        }
    }
}
