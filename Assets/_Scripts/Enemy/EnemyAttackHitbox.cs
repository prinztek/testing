using UnityEngine;
using System.Collections.Generic;

public class EnemyAttackHitbox : MonoBehaviour
{
    private EnemyStatsNew enemyStatsNew;
    private EnemyStats enemyStats;

    private HashSet<CharacterStats> alreadyHit = new HashSet<CharacterStats>();

    private void Awake()
    {
        enemyStatsNew = GetComponentInParent<EnemyStatsNew>();
        enemyStats = GetComponentInParent<EnemyStats>();
    }

    private void OnEnable()
    {
        alreadyHit.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Hurtbox"))
            return;

        CharacterStats playerStats = other.GetComponentInParent<CharacterStats>();
        if (playerStats == null)
            return;

        if (alreadyHit.Contains(playerStats))
            return;

        alreadyHit.Add(playerStats);

        int damage = GetDamage();
        if (damage > 0)
        {
            playerStats.TakeDamage(damage, transform.root.position);
        }
    }

    private int GetDamage()
    {
        if (enemyStatsNew != null)
            return enemyStatsNew.damage;

        if (enemyStats != null)
            return enemyStats.GetAttackDamage();

        return 0;
    }
}
