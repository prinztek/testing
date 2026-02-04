using UnityEngine;


public class FallingSpikeHazard : MonoBehaviour
{
    // --- Inspector Settings ---
    [Header("Trigger")]
    [Tooltip("The zone that detects the player. Assign a Box Collider2D set as a Trigger here.")]
    public Collider2D triggerZone;

    [Header("Fall Settings")]
    public float fallAcceleration = 12f;       // How fast the spike picks up speed (like gravity but tunable)
    public float warningDuration = 0.6f;       // Seconds the warning flash plays before it drops

    [Header("Reset Settings")]
    public float resetDelay = 2f;              // Seconds the spike stays landed before retracting
    public float retractSpeed = 8f;            // Initial upward speed when retracting

    [Header("Damage")]
    public int damage = 1;                     // How much damage it deals on hit
    public string playerTag = "Player";        // Tag on your player GameObject

    // --- Internal State ---
    private enum SpikeState { Idle, Warning, Falling, Landed, Resetting }
    private SpikeState state = SpikeState.Idle;

    private Vector2 startPosition;             // Where the spike retracts back to
    private float velocity = 0f;               // Current fall/retract velocity
    private float timer = 0f;                  // Used for warning and reset delays

    // --- Optional: Warning Flash Visual ---
    [Header("Visual (Optional)")]
    [Tooltip("If assigned, this SpriteRenderer will flash red during the warning phase.")]
    public SpriteRenderer spriteRenderer;
    public Color normalColor = Color.white;
    public Color warningColor = new Color(1f, 0.2f, 0.2f);
    private float flashInterval = 0.1f;        // How fast it flashes

    // --- Optional: Particles ---
    [Header("Particles (Optional)")]
    [Tooltip("Particle system that plays when the spike slams into a surface.")]
    public ParticleSystem landParticles;

    private void Awake()
    {
        startPosition = transform.position;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        switch (state)
        {
            case SpikeState.Idle:
                // Nothing happens until the trigger zone detects the player (see OnTriggerEnter2D)
                break;

            case SpikeState.Warning:
                HandleWarning();
                break;

            case SpikeState.Falling:
                HandleFalling();
                break;

            case SpikeState.Landed:
                HandleLanded();
                break;

            case SpikeState.Resetting:
                HandleResetting();
                break;
        }
    }

    // ---------------------------------------------------------------
    // TRIGGER: Player enters the detection zone -> start warning
    // ---------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && state == SpikeState.Idle)
        {
            state = SpikeState.Warning;
            timer = warningDuration;
        }
    }

    // ---------------------------------------------------------------
    // STATE: Warning (flash before dropping)
    // ---------------------------------------------------------------
    private void HandleWarning()
    {
        timer -= Time.deltaTime;

        // Flash the sprite
        if (spriteRenderer != null)
        {
            float t = Mathf.Sin(Time.time / flashInterval * Mathf.PI * 2f);
            spriteRenderer.color = t > 0 ? warningColor : normalColor;
        }

        // Warning is over -> start falling
        if (timer <= 0f)
        {
            velocity = 0f;
            state = SpikeState.Falling;
        }
    }

    // ---------------------------------------------------------------
    // STATE: Falling (accelerates downward, checks for surfaces)
    // ---------------------------------------------------------------
    private void HandleFalling()
    {
        velocity += fallAcceleration * Time.deltaTime;

        Vector2 newPos = transform.position;
        newPos.y -= velocity * Time.deltaTime;
        transform.position = newPos;

        // Raycast downward from the bottom of the spike to detect surfaces
        // Uses a small box cast matching the spike's width for accuracy
        Vector2 size = GetSpikeBounds();
        Vector2 origin = (Vector2)transform.position + new Vector2(0f, -size.y / 2f);

        RaycastHit2D hit = Physics2D.BoxCast(origin, new Vector2(size.x, 0.05f), 0f, Vector2D.down, velocity * Time.deltaTime + 0.1f);

        if (hit)
        {
            // Snap to the surface
            transform.position = new Vector2(transform.position.x, hit.point.y + size.y / 2f);
            Land();
        }
    }

    // ---------------------------------------------------------------
    // STATE: Landed (waits before retracting)
    // ---------------------------------------------------------------
    private void HandleLanded()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            velocity = -retractSpeed;   // Negative = upward
            state = SpikeState.Resetting;
        }
    }

    // ---------------------------------------------------------------
    // STATE: Resetting (floats back up to start position)
    // ---------------------------------------------------------------
    private void HandleResetting()
    {
        velocity += fallAcceleration * 0.5f * Time.deltaTime; // Decelerate as it rises

        Vector2 newPos = transform.position;
        newPos.y -= velocity * Time.deltaTime;   // velocity is negative so this moves up
        transform.position = newPos;

        // Reached or passed the start position -> snap back and go idle
        if (transform.position.y >= startPosition.y)
        {
            transform.position = startPosition;
            velocity = 0f;
            SetVisualIdle();
            state = SpikeState.Idle;
        }
    }

    // ---------------------------------------------------------------
    // COLLISION: Spike hits the player while falling or landed
    // ---------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            DamagePlayer(collision.gameObject);
        }
    }

    // ---------------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------------
    private void Land()
    {
        velocity = 0f;
        timer = resetDelay;
        state = SpikeState.Landed;

        if (landParticles != null)
            landParticles.Play();

        SetVisualIdle();
    }

    private void DamagePlayer(GameObject player)
    {
        // ---------------------------------------------------------
        // ADAPT THIS to your player's health/damage system.
        // Examples below for common setups:
        // ---------------------------------------------------------

        // Option A: If your player has a method like TakeDamage(int):
        // player.GetComponent<PlayerHealth>().TakeDamage(damage);

        // Option B: If you use a generic Health component:
        // player.GetComponent<Health>().TakeDamage(damage);

        // Option C: If you use Unity Events or a GameManager:
        // GameManager.Instance.DamagePlayer(damage);

        // Placeholder — remove and replace with your system:
        Debug.Log($"[FallingSpikeHazard] Hit player for {damage} damage!");
    }

    private void SetVisualIdle()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;
    }

    /// <summary>
    /// Returns the width and height of the spike based on its collider or sprite.
    /// </summary>
    private Vector2 GetSpikeBounds()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) return col.bounds.size;

        if (spriteRenderer != null)
            return spriteRenderer.bounds.size;

        return new Vector2(1f, 1f); // fallback
    }

    // ---------------------------------------------------------------
    // EDITOR: Draw the trigger zone and start position in the Scene view
    // ---------------------------------------------------------------
    private void OnDrawGizmos()
    {
        // Start position marker
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(startPosition != Vector2.zero ? (Vector3)startPosition : transform.position, 0.15f);

        // Trigger zone outline
        if (triggerZone != null)
        {
            Gizmos.color = new Color(1f, 0.4f, 0f, 0.3f);
            Gizmos.DrawCube(triggerZone.bounds.center, triggerZone.bounds.size);
        }
    }
}

// Quick shortcut so we don't have to type Vector2D.down
internal static class Vector2D
{
    public static Vector2 down => Vector2.down;
}
