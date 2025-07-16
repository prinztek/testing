using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private CharacterStats playerStats;  // Reference to the player’s stats

    private void Awake()
    {
        // Get the player’s CharacterStats directly from the parent (Player object)
        playerStats = transform.root.GetComponent<CharacterStats>();  // 'transform.root' gives the top-most parent (Player)

        if (playerStats == null)
        {
            Debug.LogError("PlayerCharacterStats not found!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit something tagged as "Hurtbox"
        if (other.CompareTag("Hurtbox") && playerStats != null)
        {
            Debug.Log("Hit a Hurtbox: " + other.name);
            // Access enemy stats through Hurtbox (assuming it's a child of enemy object)
            EnemyStatsNew enemyDummy = other.GetComponentInParent<EnemyStatsNew>();
            if (enemyDummy != null)
            {


                int damage = playerStats.GetDamage();
                enemyDummy.TakeDamage(damage, transform.root.position, doScreenShake: true);  // Pass the player's position for screen shake

                // Optional: Trigger any on-hit effects for the player (DoT, stun, etc.)
                playerStats.TriggerAttackHit(enemyDummy.gameObject);

            }
        }
    }
}