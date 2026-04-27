using JetBrains.Annotations;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] public GameObject hitEffectPrefab; // Effect to spawn on hit
    private Vector2 velocity;

    public int damage = 3;
    public float lifetime = 3f; // Time in seconds before projectile is automatically destroyed

    private float timer = 0f;
    private GameObject source; // Enemy who fired this projectile

    // Launch the projectile with a given velocity
    public void Launch(Vector2 velocity)
    {
        this.velocity = velocity;
        timer = 0f;

        // Rotate the projectile to face the direction it's moving
        // float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(0, 0, angle);
        // Debug.Log($"Projectile launched with velocity: {velocity}");
    }


    // Set damage and source enemy
    public void SetDamage(int damage, GameObject source)
    {
        this.damage = damage;
        this.source = source;
    }

    void Update()
    {
        // Move projectile
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Lifetime handling
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore camera bounds
        if (other.name.Contains("CameraBounds"))
            return;

        Debug.Log($"Projectile hit: {other}");

        var player = other.GetComponentInParent<CharacterStats>();
        var parent = other.GetComponentInParent<Transform>();
        int layer = other.gameObject.layer;

        // Only hit player hurtbox
        if (other.CompareTag("Hurtbox") && player != null)
        {

            if (player != null)
            {
                player.TakeDamage(damage, transform.position);

                // Optional: trigger enemy on-hit effects
                // if (source != null)
                // {
                //     EnemyStatsNew enemyStats = source.GetComponent<EnemyStatsNew>();
                //     if (enemyStats != null)
                //     {
                //         enemyStats.TriggerAttackHit(player.gameObject);
                //     }
                // }

                // Spawn hit effect
                GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

                // Destroy effect after some seconds
                Destroy(effect, 0.417f);

                Destroy(gameObject);
            }
        }

        // if ground layer is hit
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || parent.CompareTag("OneWayPlatform"))
        {
            // Spawn hit effect
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

            // Destroy effect after some seconds
            Destroy(effect, 0.417f);

            Destroy(gameObject);
        }

        // // Destroy the object if it hits the ground layer
        // if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        // {
        //     Destroy(gameObject);
        //     return;
        // }

        // Destroy projectile on any hit

        // Spawn hit effect
        // GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        // // Destroy effect after 2 seconds (adjust as needed)
        // Destroy(effect, 0.417f);

        // Destroy(gameObject);
    }
}

// used by rat archer enemy's arrow projectile prefab. Moves in a straight line and damages player on hit. Destroyed after a set lifetime or on collision with player.