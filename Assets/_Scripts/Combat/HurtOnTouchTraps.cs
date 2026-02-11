using UnityEngine;

public class HurtOnTouchTraps : MonoBehaviour
{
    public int trapDamage = 10;  // Trap damage value

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Hurtbox"))
            return;

        CharacterStats playerStats = collision.GetComponentInParent<CharacterStats>();
        EnemyStatsNew enemyStats = collision.GetComponentInParent<EnemyStatsNew>();

        playerStats?.TakeDamage(trapDamage, transform.position);
        enemyStats?.TakeDamage(trapDamage, transform.position);
    }

}
