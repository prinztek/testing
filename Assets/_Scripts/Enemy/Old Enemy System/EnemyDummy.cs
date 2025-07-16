using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class EnemyDummy : MonoBehaviour
{
    [Header("Dummy Settings")]
    public int maxHealth = 50;
    [SerializeField] private int currentHealth;

    [Header("Damage Settings")]
    public float invincibilityDuration = 0.03f;
    private bool isInvincible = false;
    private bool isDead = false;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    private CinemachineImpulseSource impulseSource;

    void Start()
    {
        currentHealth = maxHealth;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // ✅ Called when the player deals damage (direct hit)
    public void TakeDamage(int amount, Vector2 attackerPosition, bool doScreenShake = true)
    {
        if (isDead || isInvincible) return;

        currentHealth -= amount;
        Debug.Log($"Dummy took {amount} damage! Remaining HP: {currentHealth}");

        // Screen shake
        if (impulseSource != null)
            ScreenShakeManager.Instance.ScreenShake(transform.position, impulseSource);

        // Play hurt animation
        if (animator != null)
        {
            animator.CrossFade("enemyhurt", 0.1f);
            StartCoroutine(BackToIdleAfter(0.4f)); // Manual delay to return to idle
        }

        // Invincibility & flash
        StartCoroutine(InvincibilityFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ✅ DoT safe

    public void ApplyDot(int damagePerTick, float duration, float interval, Vector2 attackerPosition)
    {
        Debug.Log($"🔥 Dummy DoT: {damagePerTick} every {interval}s for {duration}s");
        StartCoroutine(DamageOverTime(damagePerTick, duration, interval, attackerPosition));
    }

    private IEnumerator DamageOverTime(int damagePerTick, float duration, float interval, Vector2 attackerPosition)
    {
        float elapsed = 0f;
        while (elapsed < duration && !isDead)
        {
            TakeDamage(damagePerTick, attackerPosition, doScreenShake: false); // No knockback for DoT
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }

    // invincibility flash after taking damage
    // ✅ This is a visual feedback to the player
    private IEnumerator InvincibilityFlash()
    {
        isInvincible = true;
        float elapsed = 0f;
        float flashInterval = 0.1f;

        while (elapsed < invincibilityDuration)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        isInvincible = false;
    }

    private IEnumerator BackToIdleAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isDead && animator != null)
        {
            animator.CrossFade("enemyidle", 0.1f);
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Notify the level manager
        FindFirstObjectByType<LevelManager>()?.OnEnemyDefeated();
        Debug.Log("Dummy has been defeated!");

        // Play death animation
        if (animator != null)
            animator.CrossFade("enemydead", 0.1f);

        // Stop movement
        // if (rb != null)
        //     rb.velocity = Vector2.zero;

        // Optionally disable collider here
        // Collider2D col = GetComponent<Collider2D>();
        // if (col != null)
        //     col.enabled = false;

        // Wait for animation then destroy
        StartCoroutine(DestroyAfterDelay(1f)); // adjust duration to match your "enemydead" animation
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
