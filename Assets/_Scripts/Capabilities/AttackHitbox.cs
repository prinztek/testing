using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

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
        // AncientBoss ancientBoss = other.GetComponentInParent<AncientBoss>();
        Boss2 boss = other.GetComponentInParent<Boss2>();
        DestroyableBlock destroyableBlock = other.GetComponentInParent<DestroyableBlock>();
        EnemyStats enemyStats = other.GetComponentInParent<EnemyStats>();

        // If already hit, skip
        if ((enemy != null && alreadyHit.Contains(enemy)) ||
            // (ancientBoss != null && alreadyHit.Contains(ancientBoss)) ||
            // (boss != null && alreadyHit.Contains(boss)) ||
            (destroyableBlock != null && alreadyHit.Contains(destroyableBlock)))
        {
            return;
        }

        int damage = playerStats.GetDamage();
        Vector3 hitPosition = transform.root.position;

        // Identiy if the character attacked with melee or ranged weapon
        // and what weapon if melee

        float forceX = 0.3f;
        float forceY = 0f;

        if (playerStats.currentAttackMode == CharacterStats.AttackMode.Melee)
        {
            if (playerStats.equippedMeleeWeapon.itemName == "Sword")
            {
                forceX = 1f;
                forceY = 0.5f;
            }
            else if (playerStats.equippedMeleeWeapon.itemName == null) // Fist
            {
                forceX = 0.3f;
                forceY = 0f;
            }
        }
        else if (playerStats.currentAttackMode == CharacterStats.AttackMode.Ranged)
        {
            if (playerStats.equippedRangedWeapon.itemName == "Bow")
            {
                forceX = 0.5f;
                forceY = 0f;
            }
        }


        // Apply damage to enemy
        if (enemy != null)
        {
            enemy.TakeDamage(damage, hitPosition, doScreenShake: true);
            playerStats.TriggerAttackHit(enemy.gameObject);
            alreadyHit.Add(enemy);
        }

        // Apply damage to enemy
        if (enemyStats != null)
        {
            enemyStats.TakeDamage(damage, hitPosition, doScreenShake: true, statusDamage: false, forceX: forceX, forceY: forceY);
            // 5% chance to stun
            // if (Random.value < 0.05f)
            //     enemyStats.AddStatus(new StunStatus(1.5f)); // can only apply to enemystats

            playerStats.TriggerAttackHit(enemyStats.gameObject); // enemy stats new way of applying dot
            alreadyHit.Add(enemyStats);
        }

        if (destroyableBlock != null)
        {
            destroyableBlock.TakeDamage(damage, hitPosition);
            alreadyHit.Add(destroyableBlock);
        }
    }
}
