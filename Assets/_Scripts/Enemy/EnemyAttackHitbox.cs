using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private EnemyStatsNew enemyStats;

    private void Awake()
    {
        // Change to GetComponentInParent to look for EnemyStatsNew on any parent, not just root
        enemyStats = transform.GetComponentInParent<EnemyStatsNew>();

        // Check if enemyStats was assigned correctly
        if (enemyStats == null)
        {
            Debug.LogError("EnemyStatsNew component not found on parent objects!");
        }
    }


    // Called when the enemy hitbox triggers a collision
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to a "Hurtbox" (likely the player or another enemy)
        if (other.CompareTag("Hurtbox"))
        {
            // Try to get the CharacterStats component from the parent of the other object
            CharacterStats playerStats = other.GetComponentInParent<CharacterStats>();

            if (playerStats != null && enemyStats != null)
            {
                // Apply damage to the player
                playerStats.TakeDamage(enemyStats.damage, transform.root.position);
                // Optional: Apply additional effects like knockback or status effects
            }
        }
    }
}
