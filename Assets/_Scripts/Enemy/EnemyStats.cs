using UnityEngine;
using Unity.Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EnemyStats : MonoBehaviour
{
    [Header("Component References")]
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private GameObject deathFXPrefab;
    [SerializeField] private OnHitFlashVFX onHitFlashVFX;

    [Header("Base Stats")]

    [SerializeField] private bool isBoss = false;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private int defense = 0;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Drop Item")]
    [SerializeField] public List<DropItem> dropTable;
    [SerializeField] public GameObject pickupPrefab;

    [Header("Runtime Stats")]
    public int CurrentHealth { get; private set; }
    public bool IsDead = false;

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
        if (IsDead) return;
        IsDead = true;
        CurrentHealth = 0;

        if (isBoss != true)
        {
            OnDeath?.Invoke(); // tell the enemy health bar that the enemy just died

            // Disable all colliders
            foreach (var col in GetComponents<Collider2D>())
                col.enabled = false;

            // boss handles his own death
            if (deathFXPrefab != null)
            {
                GameObject fx = Instantiate(deathFXPrefab, transform.position, Quaternion.identity);
                Destroy(fx, 0.5f);
            }
        }

        // Notify LevelManager
        UnityEngine.Object.FindFirstObjectByType<LevelManager>()?.OnEnemyDefeated(); // convert these to an event call?
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

    #region Drop Item Logic
    void InstantiateLoot(DropItem dropItem)
    {
        if (UnityEngine.Random.value <= dropItem.dropChance)
        {
            GameObject drop = Instantiate(pickupPrefab, transform.position, Quaternion.identity);

            PickupItem pickupItem = drop.GetComponent<PickupItem>();
            if (pickupItem != null)
            {
                pickupItem.itemData = dropItem.itemData;

                // Assign the icon to the child "PickupItemIcon"
                Transform iconChild = drop.transform.Find("PickupItemIcon");
                if (iconChild != null)
                {
                    SpriteRenderer iconRenderer = iconChild.GetComponent<SpriteRenderer>();
                    if (iconRenderer != null)
                    {
                        iconRenderer.sprite = dropItem.itemData.icon;
                    }
                }
            }
            else
            {
                Debug.LogWarning("PickupItem script not found on instantiated loot.");
            }
        }
    }
    #endregion
}
