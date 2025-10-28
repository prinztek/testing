using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 velocity;
    public int damage = 3;               // Damage dealt by the projectile
    public float lifetime = 3f;           // How long before the arrow self-destructs
    private float timer = 0f;
    private GameObject source;       // Optional: Reference to the shooter (e.g., player)

    // Launch the projectile with a given velocity
    public void Launch(Vector2 velocity)
    {
        this.velocity = velocity;
        timer = 0f; // reset lifetime timer on launch
        Debug.Log($"Projectile launched with velocity: {velocity}");
    }

    // Set the damage and source of the projectile
    public void SetDamage(int damage, GameObject source)
    {
        this.damage = damage;
        this.source = source;
    }

    void Update()
    {
        // Move projectile
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Increase timer and destroy after lifetime
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore collisions with other projectiles, or maybe your player
        if (other.CompareTag("Player") || other.name.Contains("CameraBounds"))
            return;

        // Debug.Log($"Arrow hit: {other.name} | Tag: {other.tag}");

        // Damage enemy if hit Hurtbox
        if (other.CompareTag("Hurtbox"))
        {
            EnemyStatsNew enemy = other.GetComponentInParent<EnemyStatsNew>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, transform.position, doScreenShake: true);
                // Debug.Log($"Dealt {damage} damage to {enemy.name}");

                // Optional: trigger on-hit effects (e.g., lifesteal, debuffs) from shooter
                if (source != null)
                {
                    CharacterStats stats = source.GetComponent<CharacterStats>();
                    if (stats != null)
                    {
                        stats.TriggerAttackHit(enemy.gameObject);
                    }
                }
            }

            Boss2 boss = other.GetComponentInParent<Boss2>();
            if (boss != null)
            {
                // Debug.Log("Player Transform:" + transform.root.position);
                boss.TakeDamage(damage, transform.root.position, doScreenShake: true);  // Pass the player's position for screen shake

                // Optional: Trigger any on-hit effects for the player (DoT, stun, etc.)
                // playerStats.TriggerAttackHit(enemyDummy.gameObject);

            }
        }

        // Optionally: play impact effect here before destroying

        // Destroy projectile on any hit
        Destroy(gameObject);
    }
}
