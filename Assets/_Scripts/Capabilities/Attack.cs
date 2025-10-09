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

    [Header("Input Buffering")]
    [SerializeField, Range(0f, 0.5f)] private float inputBufferDuration = 0.2f;
    private float lastBufferedInputTime = -1f;

    [Header("Post-Combo Cooldown")]
    [SerializeField] private float postComboCooldown = 0.05f;

    [Header("Attack Base Damage")]
    [SerializeField] private int baseAttack = 1;
    [SerializeField] private float nudgeForce = 5f; // Tune this for desired push

    private int attackPhase = 0;
    private float lockedUntil = 0f;
    private bool canCombo = false;
    private bool inputReady = false;
    private bool isInPostCooldown = false;
    private Ground _ground;

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
        if (stats.IsDead()) return;
        if (!inputReady || input == null || isInPostCooldown) return;
        if (hurt != null && (hurt.IsHurt() || hurt.IsInvincible())) return;
        if (!_ground.OnGround) return;

        if (input.RetrieveAttackInput())
        {
            lastBufferedInputTime = Time.time;
        }

        if (Time.time < lockedUntil) return;

        bool hasBufferedInput = Time.time - lastBufferedInputTime <= inputBufferDuration;

        if (stats.currentAttackMode == CharacterStats.AttackMode.Ranged)
        {
            if (hasBufferedInput && stats.HasRangedWeaponEquipped())
            {
                PerformRangedAttack(); // One-shot action
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
        }
    }

    private void PerformRangedAttack()
    {
        float duration = animationHandler.GetAttackAnimationLength(1, "bow");
        animationHandler.PlayAttackAnimation(1, "bow");
        lockedUntil = Time.time + duration;
        isInPostCooldown = true;
        Invoke(nameof(ResetPostCooldown), duration + postComboCooldown);
        // Debug.Log("🏹 Performed ranged attack");
    }

    private void StartAttack(int phase)
    {
        if (hurt != null && (hurt.IsHurt() || hurt.IsInvincible())) return;
        attackPhase = phase;

        string animWeapon = GetWeaponAnimType(); // "Fist" or "Sword"
        float duration = animationHandler.GetAttackAnimationLength(phase, animWeapon);
        animationHandler.PlayAttackAnimation(phase, animWeapon);

        // if (animWeapon == "Fist")
        // {
        //     nudgeForce = 2;
        // }
        // else if (animWeapon == "Sword")
        // {
        //     nudgeForce = 5;
        // }
        ApplyAttackNudge(nudgeForce);
        lockedUntil = Time.time + duration;

        if (phase == GetMaxComboPhase())
        {
            canCombo = false;
            Invoke(nameof(ResetCombo), duration + postComboCooldown);
            isInPostCooldown = true;
            Invoke(nameof(ResetPostCooldown), duration + postComboCooldown);
        }
        else
        {
            canCombo = true;
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
        lockedUntil = 0f;
        ResetCombo();
    }

    public int GetBaseAttack()
    {
        return baseAttack;
    }

    // Called by animation event
    public void EnableCombo()
    {
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

        // // Optional: zero out vertical velocity to avoid weird jumps
        rb.linearVelocity = new Vector2(0, 0);
        Vector2 newVelocity = rb.linearVelocity;
        newVelocity.y = rb.linearVelocity.y;
        newVelocity.x = direction * -nudgeForce;

        rb.linearVelocity = newVelocity;
    }

    // 🧠 Infer melee animation weapon type
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
        return IsSwordEquipped() ? 3 : 2;
    }

    private float GetCurrentComboWindow()
    {
        return IsSwordEquipped() ? swordComboWindow : fistComboWindow;
    }
}
