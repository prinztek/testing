using UnityEngine;
using Unity.Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EnemyStats : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private GameObject deathFXPrefab;
    [SerializeField] private OnHitFlashVFX onHitFlashVFX;
    [SerializeField] private GameObject hitImpactPrefab;
    [SerializeField] private AudioClip hurtSoundClip;
    [SerializeField] public GameObject shieldPrefab;
    [SerializeField] public float heightOffset = 1.5f;
    [SerializeField] private AudioClip blockedSoundClip;
    [SerializeField] private Rigidbody2D rb; // assign in inspector or via GetComponent
    // only for bosses
    [SerializeField] private GameObject bossHUD;

    [Header("Ground Check Settings:")]
    [SerializeField] public Transform groundCheckPoint; // point at which ground check happens
    [SerializeField] public float groundCheckY = 0.2f; // how far down from ground check point is Grounded() checked
    [SerializeField] public float groundCheckX = 0.5f; // how far horizontally from ground check point to the edge of the player is
    [SerializeField] private LayerMask whatIsGround; // ground layer

    #region Stats
    [Header("Base Stats")]
    [SerializeField] private bool isBoss = false;
    [SerializeField] public int maxHealth = 100;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private int defense = 0;
    [SerializeField] private float moveSpeed = 2f;

    // DEFELCT ATTACK
    [SerializeField] private bool blocksFirstHit = false;
    [SerializeField] private float blockRecoveryTime = 5.0f;
    private bool hasBlocked = false;
    private bool isRecovering = false;

    [Header("Stats Modifiers")]
    public float damageMultiplier = 1f;
    public int shieldHitsRemaining = 0;
    public float moveSpeedMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;
    public int guaranteedCrits = 0;

    #endregion

    public bool isSummon = false; // flag to indicate if this enemy is a summon

    [Header("Drop Item")]
    [SerializeField] public List<DropItem> dropTable;
    [SerializeField] public GameObject pickupPrefab;

    [Header("Runtime Stats")]
    public int CurrentHealth { get; private set; }
    public bool IsDead = false;

    // Events (FSM, UI, effects can subscribe)
    public event Action<int> OnHealthChanged;
    public event Action<EnemyStats> OnDeath;
    public event Action<int> OnDamageTaken;

    public System.Action<Vector2, float, float> OnHurt;
    // (direction, forceX, forceY)

    [Header("Status Effect and Buff Related")]
    public StatusEffect activeStatus = null;
    private Queue<StatusEffect> statusQueue = new Queue<StatusEffect>();
    internal bool canMove;
    internal bool canAttack;

    [Header("Hurt")]
    public bool isHurt;
    [HideInInspector] public Vector2 lastHitDirection;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        onHitFlashVFX = GetComponent<OnHitFlashVFX>();
        rb = GetComponent<Rigidbody2D>();

        canMove = true;
        canAttack = true;
    }

    private void Update()
    {
        if (activeStatus != null)
        {
            activeStatus.OnTick(Time.deltaTime);
            activeStatus.Update(Time.deltaTime);

            if (activeStatus.isExpired)
            {
                activeStatus.OnExpire();
                activeStatus = null;
                ResetTemporaryModifiers();
                if (statusQueue.Count > 0)
                {
                    ApplyStatus(statusQueue.Dequeue());
                }
            }
        }
    }

    private void ResetTemporaryModifiers()
    {
        damageMultiplier = 1f;
        moveSpeedMultiplier = 1f;
        shieldHitsRemaining = 0;
        attackSpeedMultiplier = 1f;
        guaranteedCrits = 0;
    }

    public void AddStatus(StatusEffect status)
    {
        if (activeStatus == null)
        {
            ApplyStatus(status);
        }
        else
        {
            statusQueue.Enqueue(status);
            // Debug.Log($" Queued status: {status.statusName}");
        }
    }

    private void ApplyStatus(StatusEffect status)
    {
        activeStatus = status;
        status.Assign(this);
        // Debug.Log($"Applied status: {status.statusName}");
    }


    #region Take Damage

    public void TakeDamage(int rawDamage, Vector2 attackerPosition, bool doScreenShake = true, bool statusDamage = false, float forceX = 0.5f, float forceY = 0f)
    {
        if (IsDead) return;

        if (CanBlock())
        {
            BlockHit();
            return;
        }

        if (bossHUD != null && bossHUD.activeSelf == false)
        {
            bossHUD.SetActive(true);
        }

        isHurt = true;
        lastHitDirection = ((Vector2)transform.position - attackerPosition).normalized;
        OnHurt?.Invoke(lastHitDirection, forceX, forceY);

        // Debug.Log("Taken Damage: " + rawDamage);

        int finalDamage = Mathf.Max(rawDamage - defense, 1);
        CurrentHealth -= finalDamage;

        LevelManager.Instance?.RegisterEnemyHit(); // this is for onscreen control grimoire highlight
        OnDamageTaken?.Invoke(finalDamage);
        OnHealthChanged?.Invoke(CurrentHealth);

        SoundFXManager.Instance.playSoundFXClilpRandomPitch(hurtSoundClip, transform, 0.05f);

        // Show floating damage
        if (DamageTextSpawner.Instance != null)
        {
            DamageTextSpawner.Instance.ShowDamage(transform.position, finalDamage, Color.red);
        }

        // VFX hit impact animations
        if (hitImpactPrefab != null)
        {
            GameObject impact = Instantiate(hitImpactPrefab, transform.position, Quaternion.identity, transform);
            Destroy(impact, 0.417f); // clean up after
        }

        if (activeStatus is BurnStatus && statusDamage == true)
        {
            // vfx for burn status effect damage
            onHitFlashVFX?.PlayOnBurnVfx();
        }
        else if (activeStatus is SlowStatus && statusDamage == true)
        {
            onHitFlashVFX?.PlayOnSlowVfx();
        }// vfx for burn status effect damage onHitFlashVFX?.PlayOnSlowVfx(); }else { // vfx for normal hit damage onHitFlashV
        else
        {
            // vfx for normal hit damage
            onHitFlashVFX.PlayOnDamageVfx();
            // Screenshake with direction
            if (doScreenShake && impulseSource != null)
            {
                Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;
                ScreenShakeManager.Instance.ScreenShake(direction, impulseSource);
            }
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public bool CanBlock()
    {
        return blocksFirstHit && !hasBlocked && !isRecovering;
    }

    public void BlockHit()
    {
        Debug.Log("Blocked Hit!");
        hasBlocked = true;
        isRecovering = true;

        // negate damage and play block effect
        // PlayBlockEffect();
        if (blockedSoundClip != null)
        {
            SoundFXManager.Instance.playSoundFXClilpRandomPitch(blockedSoundClip, transform, 0.5f);
        }

        if (shieldPrefab != null)
        {
            GameObject shield = Instantiate(shieldPrefab, transform.position + Vector3.up * heightOffset, Quaternion.identity, transform);
            Destroy(shield, 0.5f); // clean up after
        }

        StartCoroutine(BlockRecovery());
    }

    public IEnumerator BlockRecovery()
    {
        yield return new WaitForSeconds(blockRecoveryTime);
        isRecovering = false;
    }


    public void Heal(int amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }
    #region Death Logic
    private void Die()
    {
        if (IsDead) return;

        // Debug.Log("Enemy Died");

        IsDead = true;
        CurrentHealth = 0;

        // Hide boss HUD if this is a boss
        if (isBoss && bossHUD != null)
            bossHUD.SetActive(false);

        OnDeath?.Invoke(this);

        StopAllCoroutines();

        canMove = false;
        canAttack = false;

        // Stop physics
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Disable colliders
        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;

        // Spawn death FX immediately at current position
        if (deathFXPrefab != null && !isBoss)
        {
            GameObject fx = Instantiate(deathFXPrefab, transform.position, Quaternion.identity);
            Destroy(fx, 0.5f);
        }

        // Drop loot ONCE
        foreach (var dropItem in dropTable)
        {
            InstantiateLoot(dropItem);
        }

        // badge unlock trigger (if not already unlocked)
        if (!GameManager.Instance.currentData.firstKillDone)
        {
            GameManager.Instance.badgeManager.FirstKill();
        }

        // Notify LevelManager
        if (isSummon != true)
        {
            UnityEngine.Object.FindFirstObjectByType<LevelManager>()?.OnEnemyDefeated();
        }


        if (!isBoss)
            Destroy(gameObject);
    }
    #endregion
    #endregion

    #region Getters
    public Vector2 GetLastHitDirection() => lastHitDirection;
    public bool IsStunned() => !canMove || !canAttack;
    public bool IsHurt() => isHurt;
    public int GetAttackDamage() => Mathf.RoundToInt(attackDamage * damageMultiplier);
    public int GetDefense() => defense;
    public float GetMoveSpeed() => moveSpeed * moveSpeedMultiplier;
    public int GetMaxHealth() => maxHealth;

    #endregion

    #region Modifiers

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

    public bool Grounded()
    {
        if (groundCheckPoint == null) return false;

        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);
    }

    public bool HasGroundAhead(int direction)
    {
        if (groundCheckPoint == null) return false;

        Vector2 origin =
            groundCheckPoint.position +
            Vector3.right * direction * groundCheckX;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            Vector2.down,
            groundCheckY,
            whatIsGround
        );

        return hit.collider;
    }


    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.green;

        // Center ground check
        Gizmos.DrawLine(
            groundCheckPoint.position,
            groundCheckPoint.position + Vector3.down * groundCheckY
        );

        // Right ground check
        Gizmos.DrawLine(
            groundCheckPoint.position + Vector3.right * groundCheckX,
            groundCheckPoint.position + Vector3.right * groundCheckX + Vector3.down * groundCheckY
        );

        // Left ground check
        Gizmos.DrawLine(
            groundCheckPoint.position + Vector3.left * groundCheckX,
            groundCheckPoint.position + Vector3.left * groundCheckX + Vector3.down * groundCheckY
        );
    }
}
