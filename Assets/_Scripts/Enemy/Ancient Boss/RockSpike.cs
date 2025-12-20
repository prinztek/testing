using System.Collections.Generic;
using UnityEngine;

public class RockSpike : MonoBehaviour
{
    [SerializeField] private int rockSpikeDamage;
    private HashSet<CharacterStats> alreadyHit = new HashSet<CharacterStats>();
    // ✅ Auto-reset when the hitbox is enabled
    private void OnEnable()
    {
        alreadyHit.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Hurtbox")) return;

        CharacterStats playerStats = other.GetComponentInParent<CharacterStats>();
        if (playerStats == null) return; // ✅ FIX

        if (alreadyHit.Contains(playerStats)) return;

        playerStats.TakeDamage(rockSpikeDamage, transform.root.position);

        alreadyHit.Add(playerStats);
    }
}
