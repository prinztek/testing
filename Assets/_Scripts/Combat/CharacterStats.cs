using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] internal InputController input;
    [SerializeField] private OnHitFlashVFX onHitFlashVFX;
    public enum AttackMode { Melee, Ranged }
    public AttackMode currentAttackMode = AttackMode.Melee;

    [SerializeField] public AnimationHandler animationHandler;
    public BuffUIManager buffUIManager;

    [Header("Health")]
    public int maxHealth = 25;
    [SerializeField] private int currentHealth;
    public int CurrentHealth => currentHealth; // This should be the only version of the CurrentHealth property

    [Header("Base Stats")]
    public int baseDamage = 1;

    [Header("Buff Modifiers")]
    public float tempDamageMultiplier = 1f;
    public int shieldHitsRemaining = 0;
    public float moveSpeedMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;
    public int guaranteedCrits = 0;

    [Header("Equipped Weapons")]
    public GameItem equippedMeleeWeapon = null;  // null = Fist
    public GameItem equippedRangedWeapon = null; // null = no ranged
    private CinemachineImpulseSource impulseSource;
    public Buff activeBuff = null;
    private Queue<Buff> buffQueue = new Queue<Buff>();
    public delegate void AttackEvent(GameObject enemy); // This event will be triggered if an enemy is attack with fireinfuse
    public event AttackEvent OnAttackHit;
    private bool isDead = false;

    // Delegate and event for health changes
    public delegate void HealthChanged(int currentHealth);
    public event HealthChanged OnHealthChanged; // This event will be triggered whenever the character’s health changes.

    // ====================================================================================================================
    public enum SkillType // this is supposed to be something that player can unlock by defeating bosses
    {
        FireBlast,
        IceShield,
        LightningDash
    }
    // this is supposed to be something that is related to help players solve math questions easier
    // e.g., FireBlast can eliminate one wrong choice, IceShield can give a hint, LightningDash can skip question with small gold penalty

    private HashSet<SkillType> unlockedSkills = new HashSet<SkillType>();

    public void UnlockSkill(SkillType skill)
    {
        if (!unlockedSkills.Contains(skill))
        {
            unlockedSkills.Add(skill);
            // Debug.Log($"Unlocked Skill: {skill}");
        }
    }

    public bool HasSkill(SkillType skill)
    {
        return unlockedSkills.Contains(skill);
    }
    private bool isChanneling = false;
    // private float channelDuration = 0.417f; // <- length of your animation buff acquiring

    // ====================================================================================================================
    private void Awake()
    {
        onHitFlashVFX = GetComponent<OnHitFlashVFX>();

        currentHealth = maxHealth;
        // Immediately fire health change event so UI gets correct starting value
        OnHealthChanged?.Invoke(currentHealth);


        UnlockSkill(SkillType.FireBlast); // Should be calculator related
        UnlockSkill(SkillType.IceShield);
        UnlockSkill(SkillType.LightningDash);

        buffUIManager = UnityEngine.Object.FindFirstObjectByType<BuffUIManager>();
    }
    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    private void Update()
    {
        // 1️⃣ Block all input if the game is paused / a modal is open
        if (!InputGate.CanAcceptInput)
            return;

        // 2️⃣ Block input if clicking/touching UI elements (mobile or PC)
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (input.RetrieveToggleGrimoireInput())
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ToggleBook(true); // show
            }
            else
            {
                Debug.LogWarning("⚠️ UIManager.Instance not found — cannot toggle grimoire.");
            }
        }
        // Buff system
        if (activeBuff != null)
        {
            activeBuff.Update(Time.deltaTime);
            buffUIManager?.UpdateBuffSlot(activeBuff);
            if (activeBuff.isExpired)
            {
                activeBuff.OnExpire();
                buffUIManager?.RemoveBuffUI(activeBuff);
                ResetTemporaryModifiers();
                activeBuff = null;
                if (buffQueue.Count > 0) // apply next buff in queue
                {
                    ApplyBuff(buffQueue.Dequeue());
                }

            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) // toggle attack mode for testing
        {
            TryToggleAttackMode();
        }

        // if (Input.GetKeyDown(KeyCode.Z)) // toggle acquired skill 1
        // {
        //     if (HasSkill(SkillType.FireBlast))
        //     {
        //         Debug.Log("allow player to use FireBlast");
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.X)) // toggle acquired skill 2
        // {
        //     if (HasSkill(SkillType.IceShield))
        //     {
        //         Debug.Log("allow player to use IceShield");
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.C)) // toggle acquired skill 3
        // {
        //     if (HasSkill(SkillType.LightningDash))
        //     {
        //         Debug.Log("allow player to use LightningDash");
        //     }
        // }

    }

    public void TryToggleAttackMode()
    {
        if (currentAttackMode == AttackMode.Melee && equippedRangedWeapon != null)
        {
            currentAttackMode = AttackMode.Ranged;
            Debug.Log("Switched to 🏹 Ranged mode");
        }
        else
        {
            currentAttackMode = AttackMode.Melee;
            Debug.Log("Switched to ✊ Melee mode");
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"❤️ Healed for {amount}, current HP: {currentHealth}");
        OnHealthChanged?.Invoke(currentHealth); // Trigger health change event after healing
    }

    #region Weapon & Damage
    public void EquipMeleeWeapon(GameItem weapon)
    {
        if (weapon != null && weapon.itemType == ItemType.MeleeWeapon)
        {
            equippedMeleeWeapon = weapon;
            // Debug.Log($"Equipped Melee: {weapon.itemName}");
        }
    }

    public void UnequipMeleeWeapon()
    {
        equippedMeleeWeapon = null;
        // Debug.Log("Unequipped Melee Weapon (back to Fist)");
    }

    public void EquipRangedWeapon(GameItem weapon)
    {
        if (weapon != null && weapon.itemType == ItemType.RangedWeapon)
        {
            equippedRangedWeapon = weapon;
            // Debug.Log($"Equipped Ranged: {weapon.itemName}");
        }
    }

    public void UnequipRangedWeapon()
    {
        equippedRangedWeapon = null;
        // Debug.Log("Unequipped Ranged Weapon");
    }

    public bool HasRangedWeaponEquipped() => equippedRangedWeapon != null;

    public int GetDamage()
    {
        // Did you attack with meelee or ranged?
        if (currentAttackMode == AttackMode.Ranged)
        {
            int rangedWeaponBaseDamage = equippedRangedWeapon.baseDamage;
            int finalRangedDamage = Mathf.RoundToInt(rangedWeaponBaseDamage * tempDamageMultiplier);
            return finalRangedDamage;
        }
        else // Melee attack
        {
            // Fist base damage is 1 if no melee weapon is equipped
            // Otherwise, use the equipped melee weapon's base damage
            int weaponBaseDamage = (equippedMeleeWeapon != null) ? equippedMeleeWeapon.baseDamage : 1;
            int finalDamage = Mathf.RoundToInt(weaponBaseDamage * tempDamageMultiplier);
            return finalDamage;
        }
    }

    public void TriggerAttackHit(GameObject enemy)
    {
        OnAttackHit?.Invoke(enemy);

        if (activeBuff != null)
        {
            activeBuff.OnAttackHit(enemy);
        }
    }
    #endregion

    #region Buff System
    public void AddBuff(Buff buff)
    {
        if (activeBuff == null)
        {
            StartCoroutine(ChannelBuffRoutine(buff));
        }
        else
        {
            buffQueue.Enqueue(buff);
            Debug.Log($"🕓 Queued buff: {buff.GetType().Name}");
        }
    }

    private IEnumerator ChannelBuffRoutine(Buff buff)
    {
        isChanneling = true;

        // 1. Play & lock animation for EXACT time of animation
        yield return StartCoroutine(animationHandler.PlayAndLockCoroutine("buff_acquiring"));

        GameManager.Instance.BlockInput();

        ApplyBuff(buff);

        GameManager.Instance.AllowInput();

        isChanneling = false;
    }


    private void ApplyBuff(Buff buff)
    {
        ResetTemporaryModifiers();
        activeBuff = buff;
        buff.Assign(this);
        buffUIManager?.AddBuffUI(buff);
        Debug.Log($"✨ Applied buff: {buff.GetType().Name}");
    }


    public void ResetTemporaryModifiers()
    {
        tempDamageMultiplier = 1f;
        shieldHitsRemaining = 0;
        moveSpeedMultiplier = 1f;
        attackSpeedMultiplier = 1f;
        guaranteedCrits = 0;
        animationHandler.SetAnimationSpeed(1f);
    }
    #endregion

    #region Taking Damage
    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (isDead) return;

        if (shieldHitsRemaining > 0)
        {
            shieldHitsRemaining--;
            Debug.Log("🛡️ Shield absorbed the hit! Hits left: " + shieldHitsRemaining);
            return;
        }

        currentHealth -= damage;
        onHitFlashVFX.PlayOnDamageVfx();
        OnHealthChanged?.Invoke(currentHealth); // Trigger health change event

        // Screenshake direction
        if (impulseSource != null)
        {
            Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;
            ScreenShakeManager.Instance.ScreenShake(direction, impulseSource);
        }
        else
        {
            Debug.Log("Impulse Source Missing");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // attackerPosition = enemies position
            GetComponent<Hurt>().TriggerHurt(attackerPosition); // call player to get hurt
        }
    }
    #endregion

    #region Death Logic
    // ******************************** Death Handling ********************************
    public System.Action OnDeathStarted; // → fires immediately (e.g., disable input, play anim).
    public System.Action OnDeathFinished; // fires after death animation (UI + game over logic).
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        animationHandler.PlayDeadAnimation(animationHandler.GetDeathAnimationLength());
        Debug.Log("💀 Character died.");

        OnDeathStarted?.Invoke();

        // Wait until animation finishes, then call OnDeath Finished to notify listeners
        StartCoroutine(DeathAnimationCoroutine(animationHandler.GetDeathAnimationLength()));
    }

    private IEnumerator DeathAnimationCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Trigger the OnDeathFinished event for listeners (UI, etc.)
        OnDeathFinished?.Invoke();

        // Destroy the player game object
        Destroy(gameObject);
    }

    public bool IsDead() => isDead;

    #endregion
}
