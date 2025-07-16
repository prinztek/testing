using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public System.Action OnDeath;
    public enum AttackMode { Melee, Ranged }
    public AttackMode currentAttackMode = AttackMode.Melee;

    [SerializeField] private AnimationHandler animationHandler;
    public BuffUIManager buffUIManager;

    [Header("Health")]
    public int maxHealth = 100;
    public int maxMana = 100; // for future use
    public int maxStamina = 100; // for future use
    [SerializeField] private int currentHealth;
    public int CurrentHealth => currentHealth;

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

    public int gold = 0; // Player's gold for crafting
    private Buff activeBuff = null;
    private Queue<Buff> buffQueue = new Queue<Buff>();

    public delegate void AttackEvent(GameObject enemy);
    public event AttackEvent OnAttackHit;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
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

                if (buffQueue.Count > 0)
                {
                    ApplyBuff(buffQueue.Dequeue());
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) // toggle attack mode for testing
        {
            TryToggleAttackMode();
        }

    }

    private void TryToggleAttackMode()
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
    }

    public void EquipMeleeWeapon(GameItem weapon)
    {
        if (weapon != null && weapon.itemType == ItemType.MeleeWeapon)
        {
            equippedMeleeWeapon = weapon;
            Debug.Log($"Equipped Melee: {weapon.itemName}");
        }
    }

    public void UnequipMeleeWeapon()
    {
        equippedMeleeWeapon = null;
        Debug.Log("Unequipped Melee Weapon (back to Fist)");
    }

    public void EquipRangedWeapon(GameItem weapon)
    {
        if (weapon != null && weapon.itemType == ItemType.RangedWeapon)
        {
            equippedRangedWeapon = weapon;
            Debug.Log($"Equipped Ranged: {weapon.itemName}");
        }
    }

    public void UnequipRangedWeapon()
    {
        equippedRangedWeapon = null;
        Debug.Log("Unequipped Ranged Weapon");
    }

    public bool HasRangedWeaponEquipped() => equippedRangedWeapon != null;

    public int GetDamage()
    {
        int weaponBaseDamage = (equippedMeleeWeapon != null) ? equippedMeleeWeapon.baseDamage : 1;
        int finalDamage = Mathf.RoundToInt(weaponBaseDamage * tempDamageMultiplier);

        if (guaranteedCrits > 0)
            finalDamage *= 50;

        return finalDamage;
    }

    public void TriggerAttackHit(GameObject enemy)
    {
        OnAttackHit?.Invoke(enemy);

        if (activeBuff != null)
        {
            activeBuff.OnAttackHit(enemy);
        }
    }

    public void AddBuff(Buff buff)
    {
        if (activeBuff == null)
        {
            ApplyBuff(buff);
        }
        else
        {
            buffQueue.Enqueue(buff);
            Debug.Log($"🕓 Queued buff: {buff.GetType().Name}");
        }
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
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if (shieldHitsRemaining > 0)
        {
            shieldHitsRemaining--;
            Debug.Log("🛡️ Shield absorbed the hit! Hits left: " + shieldHitsRemaining);
            return;
        }

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animationHandler.PlayHurtAnimation(animationHandler.GetHurtAnimationLength());
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        animationHandler.PlayDeadAnimation(animationHandler.GetDeathAnimationLength());
        Debug.Log("💀 Character died.");
        OnDeath?.Invoke();
        StartCoroutine(AnimationCoroutine(0.833f));
    }

    private IEnumerator AnimationCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public bool IsDead() => isDead;
}
