using UnityEngine;

public class HurtOnTouchTraps : MonoBehaviour
{
    public int trapDamage = 10;  // Trap damage value
    private float lastHitTime;
    public float cooldown = 0.5f;  // Cooldown time for repeated hits

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ensure the trap doesn't hit multiple times too quickly
        if (Time.time - lastHitTime < cooldown) return;

        // Check if the object colliding is the player (or character with a Hurtbox)
        if (collision.CompareTag("Hurtbox"))
        {
            CharacterStats playerStats = collision.GetComponentInParent<CharacterStats>();
            if (playerStats != null)
            {
                lastHitTime = Time.time;

                playerStats.TakeDamage(trapDamage, transform.position);

                Debug.Log("Trap hit player, dealt damage: " + trapDamage);
            }
        }
    }
}
