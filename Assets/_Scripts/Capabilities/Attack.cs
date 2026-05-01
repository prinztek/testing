using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] internal InputController input = null;
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private Hurt hurt;
    private CharacterStats stats;
    private Rigidbody2D rb;
    private Move move; // To get direction / facing

    [Header("Weapon Combo Windows")]
    [SerializeField] private float fistComboWindow = 0.2f;
    [SerializeField] private float swordComboWindow = 0.35f;
    [SerializeField] private float airSwordComboWindow = 0.35f;

    [Header("Attack Combo Cooldowns")]
    [SerializeField] private float attackComboCooldown = 0.5f;
    [SerializeField] private float attackRangedComboCooldown = 5f;
    [SerializeField] private bool isInMeleeComboCooldown = false;
    [SerializeField] private bool isInRangedComboCooldown = false;
    [SerializeField] private int maxRangedBeforeCooldown = 3;
    [SerializeField] private float rangedChainWindow = 1.0f; // time allowed between shots
    private int rangedAttackCount = 0;
    private float lastRangedAttackTime = -999f;

    [Header("Input Buffering")]
    [SerializeField, Range(0f, 0.5f)] private float inputBufferDuration = 0.2f;
    private float lastBufferedInputTime = -1f;

    [Header("Post-Combo Cooldown")]
    [SerializeField] private float postComboCooldown = 0.05f;

    [Header("Attack Base Damage")]
    [SerializeField] private int baseAttack = 1;
    [SerializeField] private float nudgeForce = 0f; // Optional nudge on attack
    private int attackPhase = 0;
    private float lockedUntil = 0f;
    private bool canCombo = false;
    private bool inputReady = false;
    private bool isInPostCooldown = false;
    private Ground _ground;
    private bool hasAirAttacked = false;
    private bool isAirAttacking = false;
    private bool wasGrounded = true;

    [Header("Air Attack Settings")]
    [SerializeField] private float minAirTimeBeforeAirAttack = 0.12f;
    private float timeLeftGround = -999f;

    [Header("Attack Sound Clips")]
    [SerializeField] private AudioClip fistAttackClip;
    [SerializeField] private AudioClip swordAttack1Clip;
    [SerializeField] private AudioClip swordAttack2Clip;
    [SerializeField] private AudioClip swordAttack3Clip;
    [SerializeField] private AudioClip bowAttackClip;

    private void Awake()
    {
        stats = GetComponent<CharacterStats>();
        _ground = GetComponent<Ground>();
        rb = GetComponent<Rigidbody2D>();
        move = GetComponent<Move>();

    }

    private void Start()
    {
        Invoke(nameof(EnableInput), 0.1f);
    }

    private void EnableInput()
    {
        inputReady = true;
    }

    private void Update()
    {
        // Block all input if the game is paused / a modal is open
        if (!InputGate.CanAcceptInput)
            return;

        // Block input if clicking/touching UI elements (mobile or PC)
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (stats.IsDead()) return;
        if (!inputReady || input == null || isInPostCooldown) return;
        if (hurt != null && (hurt.IsHurt())) return;
        // if (hurt != null && (hurt.IsHurt() || hurt.IsInvincible())) return;

        if (stats.currentAttackMode == CharacterStats.AttackMode.Ranged)
        {
            if (isInRangedComboCooldown) return;
        }
        else
        {
            if (isInMeleeComboCooldown) return;
        }

        // if (!_ground.OnGround && stats.equippedMeleeWeapon == null) return; // Only allow attacks on ground for fist
        if (!_ground.OnGround && !IsSwordEquipped() && stats.currentAttackMode == CharacterStats.AttackMode.Melee)
            return;

        if (input.RetrieveAttackInput())
        {
            lastBufferedInputTime = Time.time;
        }

        if (Time.time < lockedUntil) return;

        bool hasBufferedInput = Time.time - lastBufferedInputTime <= inputBufferDuration;

        if (!hasBufferedInput) return;

        if (stats.currentAttackMode == CharacterStats.AttackMode.Ranged)
        {
            if (stats.HasRangedWeaponEquipped())
            {
                PerformRangedAttack();
                lastBufferedInputTime = -1f;
            }
        }
        else // Melee
        {
            if (attackPhase == 0 && hasBufferedInput && !IsAttacking())
            {
                StartAttack(1);
                lastBufferedInputTime = -1f;
            }
            else if (canCombo && attackPhase == 1 && hasBufferedInput)
            {
                StartAttack(2);
                lastBufferedInputTime = -1f;
            }
            else if (canCombo && attackPhase == 2 && hasBufferedInput && IsSwordEquipped())
            {
                StartAttack(3);
                lastBufferedInputTime = -1f;
            }

            bool hasBeenInAirLongEnough = (Time.time - timeLeftGround) >= minAirTimeBeforeAirAttack;

            if (!_ground.OnGround && !hasAirAttacked && !isAirAttacking && IsSwordEquipped()
                && CharacterStats.AttackMode.Melee == stats.currentAttackMode
                && hasBufferedInput && hasBeenInAirLongEnough)
            {
                StartAirAttack();
                lastBufferedInputTime = -1f;
            }
        }
    }


    private void LateUpdate()
    {
        bool isGroundedNow = _ground.OnGround;

        if (!wasGrounded && isGroundedNow)
        {
            // Just landed
            hasAirAttacked = false;
            isAirAttacking = false;
        }

        if (wasGrounded && !isGroundedNow)
        {
            // Just left the ground — record the time
            timeLeftGround = Time.time;
        }

        wasGrounded = isGroundedNow;
    }

    private void StartAttack(int phase)
    {
        if (hurt != null && (hurt.IsHurt() || hurt.IsInvincible())) return;
        attackPhase = phase;

        string animWeapon = GetWeaponAnimType(); // "Fist" or "Sword"
        float duration = animationHandler.GetAttackAnimationLength(phase, animWeapon);
        duration /= stats.attackSpeedMultiplier;
        animationHandler.PlayAttackAnimation(phase, animWeapon, !_ground.OnGround); // handles only the animation

        // ApplyAttackNudge(nudgeForce);
        lockedUntil = Time.time + duration;

        if (phase == GetMaxComboPhase())
        {
            // normal attacks
            canCombo = false;
            Invoke(nameof(ResetCombo), duration + postComboCooldown);
            isInPostCooldown = true;
            Invoke(nameof(ResetPostCooldown), duration + postComboCooldown);

            // enter combo cooldown AFTER animation finishes
            StartCoroutine(ComboCooldownAfterDelay(duration));
        }
        else
        {
            canCombo = true;
        }

        if (animWeapon == "Sword" && phase == 1)
        {
            SoundFXManager.Instance.playSoundFXClilpRandomPitch(swordAttack1Clip, transform, 0.1f);
        }
        else if (animWeapon == "Sword" && phase == 2)
        {
            SoundFXManager.Instance.playSoundFXClilpRandomPitch(swordAttack2Clip, transform, 0.1f);

        }
        else if (animWeapon == "Sword" && phase == 3)
        {
            SoundFXManager.Instance.playSoundFXClilpRandomPitch(swordAttack3Clip, transform, 0.1f);

        }

        if (animWeapon == "Fist")
        {
            SoundFXManager.Instance.playSoundFXClilpRandomPitch(fistAttackClip, transform, 0.2f);
        }
    }

    // only allow the character to air attack after a certain time after leaving the ground
    private void StartAirAttack()
    {
        hasAirAttacked = true;
        isAirAttacking = true;

        string animWeapon = GetWeaponAnimType();
        float duration = 0.333f; // animation duraction
        duration /= stats.attackSpeedMultiplier;

        animationHandler.PlayAttackAnimation(1, animWeapon, true);

        // ApplyAttackNudge(nudgeForce);

        lockedUntil = Time.time + duration;
        isInPostCooldown = true;

        Invoke(nameof(EndAirAttack), duration + postComboCooldown);

        if (animWeapon == "Sword")
        {
            SoundFXManager.Instance.playSoundFXClilpRandomPitch(swordAttack1Clip, transform, 0.2f);
        }
        else if (animWeapon == "Bow")
        {
            SoundFXManager.Instance.playSoundFXClilpRandomPitch(bowAttackClip, transform, 0.2f);
        }

    }
    private void EndAirAttack()
    {
        isAirAttacking = false;
        isInPostCooldown = false;
        ResetCombo();
    }
    private void PerformRangedAttack()
    {
        float now = Time.time;

        if (now - lastRangedAttackTime > rangedChainWindow)
        {
            rangedAttackCount = 0;
        }

        // Count arrow shot
        rangedAttackCount++;
        lastRangedAttackTime = now;

        float duration = animationHandler.GetAttackAnimationLength(1, "bow");
        duration /= stats.attackSpeedMultiplier;

        animationHandler.PlayAttackAnimation(1, "bow");
        lockedUntil = Time.time + duration;
        isInPostCooldown = true;
        Invoke(nameof(ResetPostCooldown), duration + postComboCooldown);

        SoundFXManager.Instance.playSoundFXClilpRandomPitch(bowAttackClip, transform, 0.2f);

        // Trigger cooldown if max range attacks reached
        if (rangedAttackCount >= maxRangedBeforeCooldown)
        {
            rangedAttackCount = 0;
            StartCoroutine(EnterRangedAttackComboCooldown());
        }
    }
    private void ResetPostCooldown()
    {
        isInPostCooldown = false;
    }

    private void ResetCombo()
    {
        attackPhase = 0;
        canCombo = false;
        lockedUntil = 0f;
    }

    public bool IsInRecovery() => isInPostCooldown;
    public bool IsAttacking() => Time.time < lockedUntil;

    public void CancelAttack()
    {
        Debug.Log("Attack cancelled");
        lockedUntil = 0f;
        ResetCombo();
        rangedAttackCount = 0;
    }

    public int GetBaseAttack()
    {
        return baseAttack;
    }

    // Called by animation event
    public void EnableCombo()
    {
        if (!_ground.OnGround) return; // No combo enabling in air

        canCombo = true;
        CancelInvoke(nameof(ClearBufferedInput));
        Invoke(nameof(ClearBufferedInput), GetCurrentComboWindow());
    }

    private void ClearBufferedInput()
    {
        lastBufferedInputTime = -1f;
        ResetCombo();
    }
    private void ApplyAttackNudge(float nudgeForce)
    {
        int direction = move.FacingRight ? 1 : -1;

        // zero out vertical velocity to avoid weird jumps
        rb.linearVelocity = new Vector2(0, 0);
        Vector2 newVelocity = rb.linearVelocity;
        newVelocity.y = rb.linearVelocity.y;
        newVelocity.x = direction * nudgeForce;

        rb.linearVelocity = newVelocity;
    }

    // Infer melee animation weapon type
    private string GetWeaponAnimType()
    {
        return IsSwordEquipped() ? "Sword" : "Fist";
    }

    private bool IsSwordEquipped()
    {
        return stats.equippedMeleeWeapon != null;
    }

    private int GetMaxComboPhase()
    {
        return IsSwordEquipped() ? _ground.OnGround ? 3 : 1 : 2;
    }

    private float GetCurrentComboWindow()
    {
        return IsSwordEquipped() ? (_ground.OnGround ? swordComboWindow : airSwordComboWindow) : fistComboWindow;
    }

    // Combo Attack Cooldown
    private IEnumerator EnterAttackComboCooldown()
    {
        isInMeleeComboCooldown = true;

        yield return new WaitForSeconds(attackComboCooldown);

        isInMeleeComboCooldown = false;
    }

    private IEnumerator EnterRangedAttackComboCooldown()
    {
        isInRangedComboCooldown = true;

        yield return new WaitForSeconds(attackRangedComboCooldown);

        isInRangedComboCooldown = false;
    }

    private IEnumerator ComboCooldownAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        StartCoroutine(EnterAttackComboCooldown());
    }
}


// Notes:
// Add an attack cooldown after the last attack in the combo to prevent spamming and allow for better timing. Done via attackComboCooldown and isInAttackComboCooldown.
// Enter attack combo cooldown
// fist or punches 2 hit combo
// sword 3 hit combo on ground, 1 hit in air
// bow and arrow single attack enter cooldown after 3 repeated attacks (can be done by tracking number of consecutive ranged attacks and then applying cooldown)