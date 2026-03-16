using UnityEngine;

public class BeamHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;

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
                playerStats.TakeDamage(damage, transform.root.position);
                // Optional: Apply additional effects like knockback or status effects
            }

        }
    }
}
