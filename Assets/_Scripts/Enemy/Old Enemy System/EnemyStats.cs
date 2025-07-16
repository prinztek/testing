using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float detectionRange = 5f;

    public bool canPatrol = true;


    void Start() => currentHealth = maxHealth;

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
