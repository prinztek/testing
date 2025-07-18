using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class EnemyStatsNew : MonoBehaviour
{
    public System.Action OnDeath;

    [Header("Component References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private AudioClip hurtSoundClip;
    [SerializeField] private GameObject hitImpactPrefab;

    [Header("Enemy Stats")]
    public int maxHealth = 50;
    private int currentHealth;
    public float moveSpeed = 2f;
    public int touchDamage = 5;

    [Header("Combat & AI Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float detectionRange = 5f;
    public float invincibilityDuration = 0.03f;
    public GameObject attackCollider;
    public bool canPatrol = true;
    public bool canAttack;
    public bool canChase;
    public bool canComboAttack = false;
    public int maxComboPhase = 3;
    public int damage = 5;
    private bool isInvincible = false;
    private bool isDead = false;

    // Use Action for health change instead of UnityEvent
    public System.Action<int> OnHealthChanged; // Delegate to notify health changes

    // Exposed properties
    public bool IsDead => isDead;
    public float DetectionRange => detectionRange;
    public float AttackRange => attackRange;
    public Animator Animator => animator;

    void Start()
    {
        currentHealth = maxHealth;  // Set initial health to max health
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    /// ***********************************************************************************************************************
    /// <summary>
    /// Call this when the enemy takes damage.
    /// </summary>
    public void TakeDamage(int amount, Vector2 attackerPosition, bool doScreenShake = true)
    {
        if (isDead || isInvincible) return;

        currentHealth -= amount;  // Decrease health by the damage amount

        // Notify health bar of health change using the Action delegate
        OnHealthChanged?.Invoke(currentHealth);

        // VFX hit impact animations
        if (hitImpactPrefab != null)
        {
            GameObject impact = Instantiate(hitImpactPrefab, transform.position, Quaternion.identity, transform);
            Destroy(impact, 0.417f); // clean up after
        }

        // Screenshake direction
        if (doScreenShake && impulseSource != null)
        {
            Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;
            ScreenShakeManager.Instance.ScreenShake(direction, impulseSource);
        }


        SoundFXManager.Instance.playSoundFXClilpRandomPitch(hurtSoundClip, transform, 1f);

        var fsm = GetComponent<EnemyStateMachine>();
        fsm.lastHitDirection = ((Vector2)transform.position - attackerPosition).normalized;
        if (fsm != null && !IsDead)
        {
            fsm.SwitchState(fsm.hurtState);
        }

        StartCoroutine(InvincibilityFlash());

        if (currentHealth <= 0)
        {
            isDead = true;
            if (fsm != null)
                fsm.SwitchState(fsm.deathState);
            else
                Die(); // fallback if FSM is missing
        }
    }

    // Add this method to get the current health value
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    public int CurrentHealth => currentHealth;

    /// <summary>
    /// Flash effect after taking damage.
    /// </summary>
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

    /// <summary>
    /// Applies damage over time.
    /// </summary>
    public void ApplyDot(int damagePerTick, float duration, float interval)
    {
        StartCoroutine(DamageOverTime(damagePerTick, duration, interval));
    }

    private IEnumerator DamageOverTime(int damagePerTick, float duration, float interval)
    {
        float elapsed = 0f;
        while (elapsed < duration && !isDead)
        {
            TakeDamage(damagePerTick, transform.position, false);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }

    /// ***********************************************************************************************************************
    /// <summary>
    /// Handles death without FSM (fallback).
    /// </summary>
    private void Die()
    {
        if (isDead) return;

        isDead = true;
        FindFirstObjectByType<LevelManager>()?.OnEnemyDefeated();

        if (animator != null)
            animator.CrossFade("enemydead", 0.1f);

        StartCoroutine(DestroyAfterDelay(1f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
