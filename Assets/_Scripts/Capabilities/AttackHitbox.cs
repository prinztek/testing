using UnityEngine;
using System.Collections.Generic;

public class AttackHitbox : MonoBehaviour
{
    private CharacterStats playerStats;  // Reference to the player’s stats
    private HashSet<MonoBehaviour> alreadyHit = new HashSet<MonoBehaviour>(); // Track enemies/bosses hit this attack
    private void OnEnable()
    {
        alreadyHit.Clear();
    }
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
        if (!other.CompareTag("Hurtbox") || playerStats == null) return;

        // Try to get enemy/boss components
        EnemyStatsNew enemy = other.GetComponentInParent<EnemyStatsNew>();
        AncientBoss ancientBoss = other.GetComponentInParent<AncientBoss>();
        Boss2 boss = other.GetComponentInParent<Boss2>();
        DestroyableBlock destroyableBlock = other.GetComponentInParent<DestroyableBlock>();

        // If already hit, skip
        if ((enemy != null && alreadyHit.Contains(enemy)) ||
            (ancientBoss != null && alreadyHit.Contains(ancientBoss)) ||
            (boss != null && alreadyHit.Contains(boss)) ||
            (destroyableBlock != null && alreadyHit.Contains(destroyableBlock)))
        {
            return;
        }

        int damage = playerStats.GetDamage();
        Vector3 hitPosition = transform.root.position;

        // Apply damage to enemy
        if (enemy != null)
        {
            enemy.TakeDamage(damage, hitPosition, doScreenShake: true);
            playerStats.TriggerAttackHit(enemy.gameObject);
            alreadyHit.Add(enemy);
        }

        // Apply damage to Boss2
        if (boss != null)
        {
            boss.TakeDamage(damage, hitPosition, doScreenShake: true);
            alreadyHit.Add(boss);
        }

        // Apply damage to AncientBoss
        if (ancientBoss != null)
        {
            ancientBoss.TakeDamage(damage, hitPosition, doScreenShake: true);
            alreadyHit.Add(ancientBoss);
        }

        if (destroyableBlock != null)
        {
            destroyableBlock.TakeDamage(damage);
        }
    }
}
