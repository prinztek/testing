using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 velocity;
    public int damage = 10;               // Damage dealt by the projectile
    public float lifetime = 3f;           // How long before the arrow self-destructs
    private float timer = 0f;

    // Launch the projectile with a given velocity
    public void Launch(Vector2 velocity)
    {
        this.velocity = velocity;
        timer = 0f; // reset lifetime timer on launch
        Debug.Log($"Projectile launched with velocity: {velocity}");
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
                Debug.Log($"Dealt {damage} damage to {enemy.name}");
            }
        }

        // Optionally: play impact effect here before destroying

        // Destroy projectile on any hit
        Destroy(gameObject);
    }
}
