using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public GameObject hitEffectPrefab; // Effect to spawn on hit

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
        // if (other.CompareTag("Player") || other.name.Contains("CameraBounds"))
        if (other.name.Contains("CameraBounds"))
            return;

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

            EnemyStats enemyStats = other.GetComponentInParent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage, transform.position, doScreenShake: true);
                // Debug.Log($"Dealt {damage} damage to {enemyStats.name}");

                // Optional: trigger on-hit effects (e.g., lifesteal, debuffs) from shooter
                if (source != null)
                {
                    CharacterStats stats = source.GetComponent<CharacterStats>();
                    if (stats != null)
                    {
                        stats.TriggerAttackHit(enemyStats.gameObject);
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

            DestroyableBlock destroyableBlock = other.GetComponent<DestroyableBlock>();
            if (destroyableBlock != null)
            {
                destroyableBlock.TakeDamage(damage, transform.root.position);
            }

        }

        // calculate whether it is facing left or right based on velocity for hit effect rotation
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);
        // Spawn hit effect on any collision (except ignored ones)
        GameObject effect = Instantiate(hitEffectPrefab, transform.position, rot);


        // Destroy effect after some seconds
        Destroy(effect, 0.417f);
        // Destroy projectile on any hit
        Destroy(gameObject);
    }
}
