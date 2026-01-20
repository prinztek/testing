using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

public class AncientBoss : MonoBehaviour
{
    // ========================================
    // ENUMS
    // ========================================
    private enum BossIntent { None, Reposition, Attack }
    public enum State { Dormant, Wake, Idle, Turn, Reposition, Attack, Hit, Death, Spinning, Buff }

    // ========================================
    // SETTINGS
    // ========================================
    [Header("Movement")]
    public float repositionSpeed = 4f;
    public float repositionDuration = 0.8f;

    [Header("Timing")]
    public float wakeTime = 1.3f;
    public float idleTime = 0.6f;
    public float turnDuration = 0.4f;
    public float attackCooldown = 2f;
    public float recoveryTime = 0.6f;

    [Header("Combat")]
    public Transform centerTransform;
    public Transform rockSpikeStartTransform;
    public float detectionRange = 8f;
    public LayerMask playerLayer;

    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float rangedRange = 8f;
    [SerializeField] private float meleeAttackDuration = 1.2f;
    [SerializeField] private float rangedAttackDuration = 1.75f;
    [SerializeField] private float spinChargeDuration = 1.2f;
    [SerializeField] private float spinDuration = 3f;

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform visual;
    public GameObject AncientBossHUD;
    private CinemachineImpulseSource impulseSource;

    [Header("Sprite Offset")]
    public float flipOffsetX = -4.4f;

    [Header("Stats")]
    public int maxHealth = 200;
    [SerializeField] private int currentHealth;
    public int CurrentHealth => currentHealth;
    public int damage = 20;

    // ========================================
    // EVENTS
    // ========================================
    public delegate void HealthChanged(int currentHealth);
    public event HealthChanged OnHealthChanged;

    public delegate void DeathStarted();
    public event DeathStarted OnDeathStarted;

    // ========================================
    // ANIMATION HASHES
    // ========================================
    private static readonly int SleepHash = Animator.StringToHash("sleep");
    private static readonly int WakeHash = Animator.StringToHash("wake");
    private static readonly int IdleHash = Animator.StringToHash("idle");
    private static readonly int RunHash = Animator.StringToHash("run");
    private static readonly int MeleeAttackHash = Animator.StringToHash("meleeAttack");
    private static readonly int RangeAttackHash = Animator.StringToHash("rangeAttack");
    private static readonly int BuffHash = Animator.StringToHash("buff");
    private static readonly int SpinningChargeAttackHash = Animator.StringToHash("spinCharge");
    private static readonly int DeathHash = Animator.StringToHash("dead");
    private static readonly int TurnLeftHash = Animator.StringToHash("turnLeft");

    // ========================================
    // STATE
    // ========================================
    private State currentState;
    private float stateTimer;
    private float attackTimer;

    private bool facingRight = true;
    private bool isTurning;
    private bool isDead;

    // ========================================
    // TARGETING
    // ========================================
    private Transform player;
    private CharacterStats playerStats;

    // Reposition
    private Vector3 repositionTarget;
    private bool hasRepositionTarget;

    // Attacks
    private enum AttackType { None, Melee, Ranged, Spin }
    private AttackType currentAttack;

    // ========================================
    // SPIKE ATTACK
    // ========================================
    public GameObject rockSpikePrefab;
    public float spikeSpacing = 2f;
    public float delayBetweenSpikes = 0.3f;

    // ========================================
    // SPIN ATTACK
    // ========================================
    private float cooldownTimer = 0f;
    public float damageCooldown = 0.1f;
    [SerializeField] private Vector2 spinHitboxSize = new Vector2(5f, 3f);
    [SerializeField] private bool showSpinHitbox = true;

    // ========================================
    // HIT FEEDBACK
    // ========================================
    [Header("Hit Feedback")]
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private float shakeIntensity = 0.05f;
    [SerializeField] private OnHitFlashVFX onHitFlashVFX;
    private Coroutine shakeCoroutine;

    // ========================================
    // UNITY LIFECYCLE
    // ========================================
    private void Awake()
    {
        animator ??= GetComponentInChildren<Animator>();
        spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
        visual ??= spriteRenderer.transform;
        onHitFlashVFX ??= GetComponent<OnHitFlashVFX>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        currentHealth = maxHealth;
        ChangeState(State.Dormant);
    }

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerStats = playerObj.GetComponent<CharacterStats>();
        }
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        player = playerObj.transform;
        playerStats = player.GetComponent<CharacterStats>();
    }

    private void Update()
    {
        if (isDead || currentState == State.Dormant)
            return;

        stateTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Wake: WakeState(); break;
            case State.Idle: IdleState(); break;
            case State.Turn: TurnState(); break;
            case State.Reposition: RepositionState(); break;
            case State.Attack: AttackState(); break;
            case State.Spinning: SpinningState(); break;
            case State.Death: DeathState(); break;
        }
    }

    // ========================================
    // STATES
    // ========================================
    private void WakeState()
    {
        if (stateTimer >= wakeTime)
            ChangeState(State.Idle);
    }

    private void IdleState()
    {
        if (stateTimer < idleTime)
            return;

        if (!PlayerInRange(detectionRange))
            return;

        DecideNextAction();
    }

    private void DecideNextAction()
    {
        float dx = player.position.x - centerTransform.position.x;

        if (!IsFacing(dx))
        {
            ChangeState(State.Turn);
            return;
        }

        if (attackTimer >= attackCooldown)
            ChangeState(State.Attack);
        else
            ChangeState(State.Reposition);
    }

    private void TurnState()
    {
        if (!isTurning)
            StartCoroutine(TurnRoutine());
    }

    private IEnumerator TurnRoutine()
    {
        isTurning = true;
        yield return new WaitForSeconds(turnDuration);
        Flip();
        isTurning = false;
        ChangeState(State.Idle);
    }

    private void RepositionState()
    {
        if (!hasRepositionTarget)
            return;

        Vector3 rootTarget = repositionTarget + (transform.position - centerTransform.position);

        transform.position = Vector3.MoveTowards(
            transform.position,
            rootTarget,
            repositionSpeed * Time.deltaTime
        );

        float dist = Mathf.Abs(centerTransform.position.x - repositionTarget.x);

        if (dist < 0.05f || stateTimer >= repositionDuration)
        {
            hasRepositionTarget = false;
            ChangeState(State.Idle);
        }
    }

    private void ChooseRepositionTarget()
    {
        hasRepositionTarget = true;

        float dir = Mathf.Sign(player.position.x - centerTransform.position.x);
        float desiredDistance = UnityEngine.Random.Range(meleeRange * 1.3f, rangedRange * 0.9f);

        repositionTarget = new Vector3(
            player.position.x - dir * desiredDistance,
            centerTransform.position.y,
            centerTransform.position.z
        );

        FaceDirection(dir);
    }

    private void AttackState()
    {
        switch (currentAttack)
        {
            case AttackType.Melee:
                if (stateTimer >= meleeAttackDuration)
                    EndAttack();
                break;

            case AttackType.Ranged:
                if (stateTimer >= rangedAttackDuration)
                    EndAttack();
                break;

            case AttackType.Spin:
                if (stateTimer >= spinChargeDuration)
                    ChangeState(State.Spinning);
                break;
        }
    }

    private void EndAttack()
    {
        attackTimer = 0f;
        idleTime = recoveryTime;
        ChangeState(State.Idle);
    }

    private void ChooseAttack()
    {
        float dist = Vector3.Distance(centerTransform.position, player.position);

        if (dist <= meleeRange)
        {
            currentAttack = UnityEngine.Random.value < 0.5f ? AttackType.Melee : AttackType.Spin;
            if (currentAttack == AttackType.Melee)
                PlayAnimation(MeleeAttackHash);
        }
        else
        {
            currentAttack = AttackType.Ranged;
            PlayAnimation(RangeAttackHash);
        }
    }

    private void SpinningState()
    {
        // Charge phase
        if (stateTimer <= spinChargeDuration)
        {
            PlayAnimation(BuffHash);
            return;
        }

        // Spin damage phase
        PlayAnimation(SpinningChargeAttackHash);

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            ApplySpinDamage();
            cooldownTimer = damageCooldown;
        }

        if (stateTimer >= spinChargeDuration + spinDuration)
        {
            cooldownTimer = 0f;
            attackTimer = 0f;
            ChangeState(State.Idle);
        }
    }

    private void ApplySpinDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(centerTransform.position, spinHitboxSize, 0f, playerLayer);

        foreach (var hit in hitEnemies)
        {
            if (hit.CompareTag("Hurtbox"))
            {
                CharacterStats stats = hit.GetComponentInParent<CharacterStats>();
                if (stats != null)
                    stats.TakeDamage(damage, centerTransform.position);
            }
        }
    }

    private void DeathState()
    {
        if (AncientBossHUD != null)
            AncientBossHUD.SetActive(false);
        OnDeathStarted?.Invoke();
        isDead = true;
    }

    // ========================================
    // HELPERS
    // ========================================
    private bool PlayerInRange(float range)
    {
        return player != null &&
               Vector3.Distance(centerTransform.position, player.position) <= range;
    }

    private bool IsFacing(float xDir)
    {
        return (xDir > 0 && facingRight) || (xDir < 0 && !facingRight);
    }

    private void FaceDirection(float xDir)
    {
        if (currentState == State.Attack || currentState == State.Spinning)
            return;

        bool shouldFaceRight = xDir > 0;
        if (facingRight == shouldFaceRight)
            return;

        facingRight = shouldFaceRight;
        ApplyFlip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        ApplyFlip();
    }

    private void ApplyFlip()
    {
        Vector3 scale = visual.localScale;
        scale.x = facingRight ? 1f : -1f;
        visual.localScale = scale;

        Vector3 pos = visual.localPosition;
        pos.x = facingRight ? 0f : flipOffsetX;
        visual.localPosition = pos;
    }

    private void ChangeState(State newState)
    {
        OnStateExit(currentState);
        currentState = newState;
        stateTimer = 0f;
        OnStateEnter(newState);
    }

    private void OnStateEnter(State state)
    {
        switch (state)
        {
            case State.Dormant:
                PlayAnimation(SleepHash);
                break;
            case State.Wake:
                PlayAnimation(WakeHash);
                break;
            case State.Idle:
                PlayAnimation(IdleHash);
                break;
            case State.Reposition:
                ChooseRepositionTarget();
                PlayAnimation(RunHash);
                break;
            case State.Turn:
                PlayAnimation(TurnLeftHash);
                break;
            case State.Attack:
                ChooseAttack();
                break;
            case State.Death:
                PlayAnimation(DeathHash);
                isDead = true;
                UnityEngine.Object.FindFirstObjectByType<LevelManager>()?.OnEnemyDefeated();
                break;
        }
    }

    private void OnStateExit(State state)
    {
        // Add any cleanup logic here if needed
    }

    private void PlayAnimation(int animHash)
    {
        animator?.CrossFade(animHash, 0f, 0);
    }

    // ========================================
    // PUBLIC METHODS
    // ========================================
    public void WakeUp()
    {
        if (currentState != State.Dormant)
            return;

        ChangeState(State.Wake);
    }

    public int GetDamage()
    {
        return damage;
    }

    public void TakeDamage(int damageAmount, Vector2 attackerPosition, bool doScreenShake = true)
    {
        if (currentState == State.Dormant || isDead)
            return;

        // Visual feedback
        // if (shakeCoroutine != null)
        //     StopCoroutine(shakeCoroutine);
        // shakeCoroutine = StartCoroutine(ShakeSprite());

        onHitFlashVFX?.PlayOnDamageVfx();

        if (doScreenShake)
            StartScreenshakeFromGettingAttacked(attackerPosition);

        // Show HUD
        if (AncientBossHUD != null && !AncientBossHUD.activeSelf)
            AncientBossHUD.SetActive(true);

        // Apply damage
        currentHealth -= damageAmount;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            ChangeState(State.Death);
            playerStats?.UnlockSkill(SkillType.PermutationPulse);
        }
    }

    public void TriggerSpikeBarrage()
    {
        StartCoroutine(SpikeBarrage());
    }

    // ========================================
    // COROUTINES
    // ========================================
    private IEnumerator SpikeBarrage()
    {
        float direction = facingRight ? 1f : -1f;
        Vector3 startPos = rockSpikeStartTransform.position;

        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = startPos + Vector3.right * direction * spikeSpacing * i;
            StartCoroutine(SpawnSpike(rockSpikePrefab, pos));
            yield return new WaitForSeconds(delayBetweenSpikes);
        }
    }

    private IEnumerator SpawnSpike(GameObject spikePrefab, Vector3 position)
    {
        GameObject spike = Instantiate(spikePrefab, position, Quaternion.identity);

        RockSpike spikeComponent = spike.GetComponent<RockSpike>();
        if (spikeComponent != null)
        {
            spikeComponent.SetFacing(facingRight);
            spikeComponent.Emerge();
        }

        StartScreenshakeForAttacking();

        yield return new WaitForSeconds(1.0f);

        if (spikeComponent != null)
        {
            spikeComponent.Retract();
        }

        yield return new WaitForSeconds(0.1f);
        Destroy(spike);
    }

    private IEnumerator ShakeSprite()
    {
        if (spriteTransform == null)
            yield break;

        Vector3 originalPos = spriteTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * shakeIntensity;
            float y = UnityEngine.Random.Range(-1f, 1f) * shakeIntensity;
            spriteTransform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteTransform.localPosition = originalPos;
    }

    // ========================================
    // SCREENSHAKE
    // ========================================
    public void StartScreenshakeFromGettingAttacked(Vector2 attackerPosition)
    {
        if (impulseSource != null)
        {
            Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;
            ScreenShakeManager.Instance.ScreenShake(direction, impulseSource);
        }
    }

    public void StartScreenshakeForAttacking()
    {
        if (impulseSource != null)
        {
            ScreenShakeManager.Instance.ScreenShake(impulseSource);
        }
    }

    // ========================================
    // GIZMOS
    // ========================================
    private void OnDrawGizmosSelected()
    {
        if (centerTransform == null)
            return;

        Vector3 center = centerTransform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, detectionRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, rangedRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(center, meleeRange);

#if UNITY_EDITOR
        Handles.color = Color.white;
        Handles.Label(center + Vector3.up * 2f, $"State: {currentState}");
        Handles.color = Color.magenta;
        Handles.Label(center + Vector3.right * meleeRange, "Melee");
        Handles.color = Color.cyan;
        Handles.Label(center + Vector3.right * rangedRange, "Ranged");
#endif

        if (showSpinHitbox)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, spinHitboxSize);
        }
    }
}