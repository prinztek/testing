using UnityEngine;
using Unity.Cinemachine;
using System;

public class EnemyStats : MonoBehaviour
{
    [Header("Component References")]
    private CinemachineImpulseSource impulseSource;

    [Header("Base Stats")]

    [SerializeField] private bool isBoss = false;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private int defense = 0;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Runtime Stats")]
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    // Events (FSM, UI, effects can subscribe)
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public event Action<int> OnDamageTaken;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    #region Health

    public void TakeDamage(int rawDamage, Vector2 attackerPosition, bool doScreenShake = true)
    {
        if (IsDead) return;

        Debug.Log("Taken Damage: " + rawDamage);

        int finalDamage = Mathf.Max(rawDamage - defense, 1);
        CurrentHealth -= finalDamage;

        OnDamageTaken?.Invoke(finalDamage);
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }

        // Show floating damage
        if (DamageTextSpawner.Instance != null)
        {
            if (!doScreenShake)
            {
                DamageTextSpawner.Instance.ShowDamage(transform.position, finalDamage, Color.red);
            }
        }

        // Screenshake with direction
        if (doScreenShake && impulseSource != null)
        {
            Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;
            ScreenShakeManager.Instance.ScreenShake(direction, impulseSource);
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    private void Die()
    {
        CurrentHealth = 0;
        OnDeath?.Invoke(); // tell the enemy health bar that the enemy just died
    }

    #endregion

    #region Getters

    public int GetAttackDamage() => attackDamage;
    public int GetDefense() => defense;
    public float GetMoveSpeed() => moveSpeed;
    public int GetMaxHealth() => maxHealth;

    #endregion

    #region Modifiers (Optional Extension)

    public void ModifyAttack(int amount)
    {
        attackDamage += amount;
    }

    public void ModifyDefense(int amount)
    {
        defense += amount;
    }

    public void ModifyMoveSpeed(float amount)
    {
        moveSpeed = Mathf.Max(0f, moveSpeed + amount);
    }

    #endregion
}
