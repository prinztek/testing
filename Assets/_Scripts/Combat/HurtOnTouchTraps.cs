using UnityEngine;

public class HurtOnTouchTraps : MonoBehaviour
{
    public int trapDamage = 10;  // Trap damage value

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Trap triggered by: " + collision.name);

        // Check if the object colliding is the player (or character with a Hurtbox)
        if (collision.CompareTag("Hurtbox"))
        {
            CharacterStats playerStats = collision.GetComponentInParent<CharacterStats>();
            Hurt isInvincible = collision.GetComponentInParent<Hurt>();
            if (playerStats != null && isInvincible.IsInvincible() != true)
            {

                // Simply apply damage (invincibility handled elsewhere)
                playerStats.TakeDamage(trapDamage, transform.position);
                // Debug.Log("Trap hit player, dealt damage: " + trapDamage);
            }
        }
    }
}
