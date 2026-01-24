using UnityEngine;

public class Ground : MonoBehaviour
{
    public bool OnGround { get; private set; }
    public bool OnOneWayPlatform { get; private set; }
    public float Friction { get; private set; }

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    public PlatformEffector2D OneWayEffector { get; private set; }
    public Collider2D OneWayPlatformCollider { get; private set; }

    void FixedUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        OnGround = false;
        OnOneWayPlatform = false;
        OneWayPlatformCollider = null;

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            OnGround = true;

            if (hit.CompareTag("OneWayPlatform"))
            {
                OnOneWayPlatform = true;
                // OneWayPlatformCollider = hit;
                OneWayEffector = hit.GetComponent<PlatformEffector2D>();
                break; // IMPORTANT: only need the one we stand on
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RetrieveFriction(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        RetrieveFriction(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Friction = 0f;
    }

    private void RetrieveFriction(Collision2D collision)
    {
        if (collision.rigidbody == null) return;

        PhysicsMaterial2D mat = collision.collider.sharedMaterial;
        Friction = mat != null ? mat.friction : 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
