using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private EnemyStatsNew enemyStats;
    private void Awake()
    {
        enemyStats = transform.root.GetComponent<EnemyStatsNew>(); // 'transform.root' gives the top-most parent (Player)
    }

    // Enemy attacking the player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hurtbox"))
        {
            CharacterStats playerStats = other.GetComponentInParent<CharacterStats>();
            if (playerStats != null)
            {
                // Debug.Log("Enemy Transform:" + transform.root.position);
                // Apply knockback by sending enemy’s position
                playerStats.TakeDamage(enemyStats.damage, transform.root.position);

                // Optional: enemy-specific effects (e.g., poison, fire, etc.)
            }
        }
    }
}
