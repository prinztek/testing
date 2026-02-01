using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fallDelay = 0.5f;        // Time before platform falls
    [SerializeField] private float respawnDelay = 3f;       // Time before platform respawns
    [SerializeField] private float fallSpeed = 5f;          // How fast it falls

    [Header("Optional Shake")]
    [SerializeField] private bool shakeBeforeFall = true;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeAmount = 0.1f;

    private Vector3 originalPosition;
    private Rigidbody2D rb;
    private Collider2D platformCollider;
    private SpriteRenderer spriteRenderer;

    private bool isFalling = false;
    private bool playerOnPlatform = false;
    private float fallTimer = 0f;
    private float shakeTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalPosition = transform.position;

        // Setup rigidbody
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        if (isFalling) return;

        if (playerOnPlatform)
        {
            fallTimer += Time.deltaTime;

            // Shake effect
            if (shakeBeforeFall && fallTimer < fallDelay)
            {
                shakeTimer += Time.deltaTime;
                float shakeProgress = fallTimer / fallDelay;
                float currentShake = shakeAmount * shakeProgress;

                transform.position = originalPosition +
                    new Vector3(
                        Mathf.Sin(shakeTimer * 30f) * currentShake,
                        0f,
                        0f
                    );
            }

            // Start falling
            if (fallTimer >= fallDelay)
            {
                StartFalling();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = true;
            fallTimer = 0f;
            shakeTimer = 0f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;

            // Reset shake
            if (!isFalling)
            {
                transform.position = originalPosition;
                fallTimer = 0f;
            }
        }
    }

    private void StartFalling()
    {
        isFalling = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallSpeed;

        // Respawn after delay
        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        // Reset everything
        transform.position = originalPosition;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        isFalling = false;
        playerOnPlatform = false;
        fallTimer = 0f;
        shakeTimer = 0f;

        // Re-enable collider if it was disabled
        if (platformCollider != null)
        {
            platformCollider.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = Application.isPlaying ? originalPosition : transform.position;
        Gizmos.DrawWireCube(pos, transform.localScale);
    }
}