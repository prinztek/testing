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
                playerStats.TakeDamage(enemyStats.touchDamage);
                Debug.Log("✅ Player took contact damage!");
            }
        }
    }
}
