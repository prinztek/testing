using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector2 velocity;

    public int damage = 3;
    public float lifetime = 3f;

    private float timer = 0f;
    private GameObject source; // Enemy who fired this projectile

    // Launch the projectile with a given velocity
    public void Launch(Vector2 velocity)
    {
        this.velocity = velocity;
        timer = 0f;
        Debug.Log($"Projectile launched with velocity: {velocity}");
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

        // Only hit player hurtbox
        if (other.CompareTag("Hurtbox"))
        {
            CharacterStats player = other.GetComponentInParent<CharacterStats>();
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
            }

            Destroy(gameObject);
        }
    }
}
