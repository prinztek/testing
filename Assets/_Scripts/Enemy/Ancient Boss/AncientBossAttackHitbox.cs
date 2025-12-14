using UnityEngine;
using System.Collections.Generic;

public class AncientBossAttackHitbox : MonoBehaviour
{
    [Header("References")]
    public AncientBoss ancientBoss; // Assign this in the inspector or dynamically in code

    [Header("Settings")]
    private HashSet<CharacterStats> alreadyHit = new HashSet<CharacterStats>();

    private void Awake()
    {
        // Automatically get AncientBoss if not assigned manually
        if (ancientBoss == null)
            ancientBoss = GetComponentInParent<AncientBoss>();
    }

    // ✅ Auto-reset when the hitbox is enabled
    private void OnEnable()
    {
        alreadyHit.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Hurtbox")) return;

        CharacterStats playerStats = other.GetComponentInParent<CharacterStats>();
        if (playerStats == null || ancientBoss == null) return;

        if (alreadyHit.Contains(playerStats)) return;

        // Apply the boss’s damage to the player
        playerStats.TakeDamage(ancientBoss.GetDamage(), transform.root.position);

        alreadyHit.Add(playerStats);
    }
}
