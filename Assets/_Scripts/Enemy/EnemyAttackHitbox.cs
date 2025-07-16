using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    private Transform enemyTransform;

    private void Awake()
    {
        enemyTransform = transform.parent?.parent;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hurtbox"))
        {
            CharacterStats playerStats = other.GetComponentInParent<CharacterStats>();
            if (playerStats != null)
            {
                Debug.Log($"Attacked by SLIME === {other.name} for {damage} damage");
                playerStats.TakeDamage(damage);

                // Optional: enemy-specific on-hit logic (e.g., apply status)
            }
        }
    }
}
