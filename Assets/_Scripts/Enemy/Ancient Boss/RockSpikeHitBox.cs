using System.Collections.Generic;
using UnityEngine;

public class RockSpikeHitBox : MonoBehaviour
{
    [SerializeField] private int damage;
    private HashSet<CharacterStats> alreadyHit = new HashSet<CharacterStats>();
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        DisableHitbox();
    }

    private void OnEnable()
    {
        alreadyHit.Clear();
    }

    public void EnableHitbox()
    {
        alreadyHit.Clear();
        col.enabled = true;
    }

    public void DisableHitbox()
    {
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Hurtbox")) return;

        CharacterStats stats = other.GetComponentInParent<CharacterStats>();
        if (stats == null) return;

        if (alreadyHit.Contains(stats)) return;

        stats.TakeDamage(damage, transform.root.position);
        alreadyHit.Add(stats);
    }
}
