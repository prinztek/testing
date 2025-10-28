// Bullet
using UnityEngine;


public class RoundRedBullet : MonoBehaviour
{
    public float bulletLife = 1f;  // Defines how long before the bullet is destroyed
    public float rotation = 0f;
    public float speed = 1f;


    private Vector2 spawnPoint;
    private float timer = 0f;
    // damage for bullet projectiles for now - later used by attacks of boss enemies
    [SerializeField] private int bulletDamage = 10;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = new Vector2(transform.position.x, transform.position.y);
    }


    // Update is called once per frame
    void Update()
    {
        if (timer > bulletLife) Destroy(this.gameObject);
        timer += Time.deltaTime;
        transform.position = Movement(timer);
    }


    private Vector2 Movement(float timer)
    {
        // Moves right according to the bullet's rotation
        float x = timer * speed * transform.right.x;
        float y = timer * speed * transform.right.y;
        return new Vector2(x + spawnPoint.x, y + spawnPoint.y);
    }

    // Called when the enemy hitbox triggers a collision
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to a "Hurtbox" (likely the player or another enemy)
        if (other.CompareTag("Hurtbox"))
        {
            // Try to get the CharacterStats component from the parent of the other object
            CharacterStats playerStats = other.GetComponentInParent<CharacterStats>();

            if (playerStats != null)
            {
                // Apply damage to the player
                playerStats.TakeDamage(bulletDamage, transform.root.position);
                // Optional: Apply additional effects like knockback or status effects
            }

        }
    }
}
