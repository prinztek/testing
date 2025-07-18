using UnityEngine;

public class HurtOnTouch : MonoBehaviour
{
    private EnemyStatsNew enemyStats;
    private float lastHitTime;
    public float cooldown = 0.5f;

    private void Awake()
    {
        enemyStats = GetComponentInParent<EnemyStatsNew>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time - lastHitTime < cooldown) return;
        if (collision.CompareTag("Hurtbox"))
        {
            CharacterStats playerStats = collision.GetComponentInParent<CharacterStats>();
            if (playerStats != null)
            {
                lastHitTime = Time.time;
                Debug.Log("Enemy Transform:" + transform.root.position);
                // Pass the enemy's position to apply directional knockback to player
                playerStats.TakeDamage(enemyStats.touchDamage, transform.root.position);

                // Debug.Log("✅ Player took contact damage with knockback!");
            }
        }
    }
}
