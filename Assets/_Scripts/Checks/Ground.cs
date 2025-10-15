using UnityEngine;

public class Ground : MonoBehaviour
{
    public bool OnGround { get; private set; }
    public float Friction { get; private set; }

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    public MovingPlatform CurrentPlatform { get; private set; }

    private void Update()
    {
        // Check if player is on the ground using the OverlapCircle method
        OnGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RetrieveFriction(collision);
        TrySetCurrentPlatform(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        RetrieveFriction(collision);
        TrySetCurrentPlatform(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Friction = 0f;
        if (collision.gameObject.GetComponent<MovingPlatform>() == CurrentPlatform)
        {
            CurrentPlatform = null;
        }
    }

    private void TrySetCurrentPlatform(Collision2D collision)
    {
        var platform = collision.gameObject.GetComponent<MovingPlatform>();
        if (platform != null)
        {
            CurrentPlatform = platform;
        }
    }

    private void RetrieveFriction(Collision2D collision)
    {
        if (collision.rigidbody == null) return;

        PhysicsMaterial2D mat = collision.rigidbody.sharedMaterial;
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
