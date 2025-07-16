using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyBase : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;
    private bool isDead = false;

    public Animator animator;
    public Rigidbody2D rb;
    public Collider2D col;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        animator?.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        animator?.SetTrigger("Die");
        rb.simulated = false;
        col.enabled = false;

        FindFirstObjectByType<LevelManager>()?.OnEnemyDefeated();

        Destroy(gameObject, 1.2f); // Wait for animation
    }
}
