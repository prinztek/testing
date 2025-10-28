using UnityEngine;
using System.Collections.Generic;

public class BossAttackHitbox : MonoBehaviour
{
    [Header("References")]
    public Boss2 boss2; // Assign this in the inspector or dynamically in code

    [Header("Settings")]
    private HashSet<CharacterStats> alreadyHit = new HashSet<CharacterStats>();

    private void Awake()
    {
        // Automatically get Boss2 if not assigned manually
        if (boss2 == null)
            boss2 = GetComponentInParent<Boss2>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Hurtbox")) return;

        CharacterStats playerStats = other.GetComponentInParent<CharacterStats>();
        if (playerStats == null || boss2 == null) return;

        if (alreadyHit.Contains(playerStats)) return;

        // Apply the boss’s damage to the player
        playerStats.TakeDamage(boss2.attackDamage, transform.root.position);

        alreadyHit.Add(playerStats);
    }

    /// <summary>
    /// Call this at the start of each attack animation
    /// to allow the boss to hit targets again.
    /// </summary>
    public void ResetHits() => alreadyHit.Clear();
}
