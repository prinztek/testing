using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    // ========================================
    // REFERENCES
    // ========================================
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CircleCollider2D circle2d;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // ========================================
    // EXPLOSION SETTINGS
    // ========================================
    [Header("Explosion")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float explosionRadius = 2f;

    // ========================================
    // VISUALS
    // ========================================
    [Header("Visuals")]
    [SerializeField] private float rotationSpeed = 360f;

    // ========================================
    // LAUNCH DATA
    // ========================================
    [Header("Launch Settings")]
    [SerializeField] private float height = 3f; // Arc height for calculated launch

    private Vector3 sourcePosition;
    private Vector3 targetPosition;
    private Vector2 launchVelocity;
    private Vector2 bombDirection;
    private float gravity;

    // ========================================
    // DATA
    // ========================================
    private int damage;
    private GameObject owner;
    private bool hasExploded = false;

    // ========================================
    // UNITY
    // ========================================
    private void Awake()
    {
        rb ??= GetComponent<Rigidbody2D>();
        circle2d ??= GetComponent<CircleCollider2D>();
        spriteRenderer ??= GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (hasExploded) return;

        // Rotate while flying
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }

    // ========================================
    // SETUP
    // ========================================
    public void SetDamage(int damageAmount, float radius, GameObject ownerObject)
    {
        damage = damageAmount;
        explosionRadius = radius;
        owner = ownerObject;

        // Ignore collision with owner
        if (owner != null && circle2d != null)
        {
            Collider2D[] ownerColliders = owner.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D col in ownerColliders)
            {
                Physics2D.IgnoreCollision(circle2d, col);
            }
        }
    }

    public void SetupLaunch(Vector3 source, Vector3 target, float arcHeight)
    {
        sourcePosition = source;
        targetPosition = target;
        height = arcHeight;
    }

    public void SetupSimpleLaunch(Vector2 velocity, Vector2 direction)
    {
        launchVelocity = velocity;
        bombDirection = direction;
    }

    // Launch the bomb - has two scenarios:
    // 1st case - calculateLaunch = true: velocity is calculated to hit targetPosition
    //            uses kinematic equation with gravity and arc height
    //            (this is for enemy bombers aiming at player)
    public void Launch(bool calculateLaunch = true)
    {
        // Make the bomb solid and have a dynamic rigidbody
        circle2d.isTrigger = false;
        rb.bodyType = RigidbodyType2D.Dynamic;

        if (calculateLaunch)
        {
            // Launch bomb to target using calculated arc
            if (gravity == 0) gravity = Physics2D.gravity.y;

            Vector2 calculatedVelocity = CalculateLaunchVelocity(sourcePosition, targetPosition, height, gravity);
            rb.linearVelocity = calculatedVelocity;
        }
        else
        {
            // No target set - use simple launch velocity instead
            Vector2 velocity = launchVelocity;
            velocity.x *= bombDirection.x;
            rb.AddForce(velocity, ForceMode2D.Impulse);
        }
    }

    // Calculate initial velocity needed to reach target with given arc height
    private Vector2 CalculateLaunchVelocity(Vector3 source, Vector3 target, float arcHeight, float gravity)
    {
        float displacementY = target.y - source.y;
        Vector3 displacementXZ = new Vector3(target.x - source.x, 0, 0);

        float time = Mathf.Sqrt(-2 * arcHeight / gravity) + Mathf.Sqrt(2 * (displacementY - arcHeight) / gravity);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * arcHeight);
        Vector3 velocityXZ = displacementXZ / time;

        return velocityXZ + velocityY;
    }

    // ========================================
    // COLLISION
    // ========================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded) return;

        // Explode on hitting anything (ground, player, walls, etc.)
        Explode();
    }

    // ========================================
    // EXPLOSION
    // ========================================
    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // Play explosion effect
        if (explosionEffectPrefab != null)
        {
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosionEffect, 0.5f); // Destroy effect after 0.5 seconds
        }

        // Deal damage in radius
        DealExplosionDamage();

        // Destroy the bomb
        if (Application.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    private void DealExplosionDamage()
    {
        // Find all colliders in explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            // Don't damage the owner
            if (hit.gameObject == owner) continue;

            // Try to damage player
            CharacterStats player = hit.GetComponent<CharacterStats>();
            if (player != null)
            {
                player.TakeDamage(damage, transform.position);

                // Apply knockback
                Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                Rigidbody2D playerRb = hit.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.AddForce(knockbackDir * 8f + Vector2.up * 3f, ForceMode2D.Impulse);
                }
            }

            // Try to damage destructibles
            IDestructible destructible = hit.GetComponent<IDestructible>();
            if (destructible != null)
            {
                destructible.TakeDamage(damage);
            }
        }
    }

    // ========================================
    // DEBUG
    // ========================================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

public interface IDestructible
{
    void TakeDamage(int damage);
}