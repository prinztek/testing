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
    public enum State { Dormant, Wake, Idle, Patrol, Turn, Chase, Attack, Hit, Death, Spinning, Buff }

    // ========================================
    // SETTINGS
    // ========================================
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float patrolDistance = 8f;

    [Header("Timing")]
    public float wakeTime = 1.3f;
    public float idleTime = 1.5f;
    public float turnDuration = 0.5f;
    public float preTurnIdleTime = 0.5f;
    public float postTurnIdleTime = 0.5f;
    public float attackCooldown = 2f;

    [Header("Combat")]
    public Transform centerTransform; // ancient boss center mass
    public Transform rockSpikeStartTransform; // ancient boss center mass
    public float detectionRange = 6f; // Distance to detect player
    public LayerMask playerLayer;
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float rangedRange = 8f;
    [SerializeField] private float meleeAttackDuration = 1.2f;
    [SerializeField] private float rangedAttackDuration = 1.75f;
    [SerializeField] private float spinChargeDuration = 4.64f;

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform visual;
    public GameObject AncientBossHUD;
    private CinemachineImpulseSource impulseSource;

    [Header("Offset Compensation")]
    [Tooltip("Adjust this to compensate for off-center sprite pivot")]
    public float flipOffsetX = -4.4f;

    [Header("Ancient Boss Stats")]
    public int maxHealth = 200;
    [SerializeField] private int currentHealth;
    public int CurrentHealth => currentHealth;

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
    // STATE VARIABLES
    // ========================================
    private State currentState;
    private float stateTimer;

    // Movement
    private Vector3 leftBound;
    private Vector3 rightBound;
    private bool facingRight = true;
    private bool movingRight = true;
    private bool wantsToFaceRight;

    // Combat
    private Transform player;
    private CharacterStats playerStats;
    private float attackTimer;
    private bool isDead = false;
    private bool isTurning = false;

    public int damage = 20;
    public int GetDamage() { return damage; }

    private enum AttackType { None, Melee, Ranged, Spin }
    private AttackType currentAttack;

    [Header("Hit Feedback")]
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private float shakeIntensity = 0.05f;
    [SerializeField] private OnHitFlashVFX onHitFlashVFX;
    private Coroutine shakeCoroutine;

    // ========================================
    // INITIALIZATION
    // ========================================
    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (visual == null) visual = spriteRenderer.transform;
        onHitFlashVFX = GetComponent<OnHitFlashVFX>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        // Set patrol bounds
        Vector3 startPos = transform.position;
        leftBound = startPos + Vector3.left * patrolDistance;
        rightBound = startPos + Vector3.right * patrolDistance;

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
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        if (playerObj != null) playerStats = player.GetComponent<CharacterStats>();
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        player = playerObj.transform;
        playerStats = player.GetComponent<CharacterStats>();
    }

    // ========================================
    // UPDATE
    // ========================================
    private void Update()
    {
        if (isDead) return;

        // Boss does NOTHING while dormant // sleeping
        if (currentState == State.Dormant)
            return;

        stateTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Wake: WakeState(); break;
            case State.Idle: IdleState(); break;
            case State.Turn: TurnState(); break;
            case State.Patrol: PatrolState(); break;
            case State.Chase: ChaseState(); break;
            case State.Attack: AttackState(); break;
            case State.Death: DeathState(); break;
            case State.Spinning: SpinningState(); break;
        }
    }



    #region STATES
    private void WakeState()
    {
        if (stateTimer >= wakeTime)
            ChangeState(State.Idle);
    }

    private void IdleState()
    {
        if (stateTimer >= idleTime)
        {
            if (PlayerInRange(detectionRange))
            {
                ChangeState(State.Chase);
            }
            else
            {
                ChangeState(State.Patrol);
            }
        }
    }

    private void TurnState()
    {
        StartCoroutine(HandleTurn());
    }

    private IEnumerator HandleTurn()
    {
        if (isTurning) yield break;
        isTurning = true;

        // Play turn animation
        // PlayAnimation(TurnLeftHash);
        yield return new WaitForSeconds(turnDuration);

        // Flip facing direction
        Flip();

        // Update moving direction to match new facing
        movingRight = facingRight;

        isTurning = false;

        // Decide next state
        if (PlayerInRange(detectionRange))
            ChangeState(State.Chase);
        else
            ChangeState(State.Patrol);
    }

    private void PatrolState()
    {
        if (PlayerInRange(detectionRange))
        {
            ChangeState(State.Chase);
            return;
        }

        Vector3 target = movingRight ? rightBound : leftBound;
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            ChangeState(State.Turn);
            return;
        }
    }

    private void AttackState()
    {
        switch (currentAttack)
        {
            case AttackType.Melee:
                if (stateTimer >= meleeAttackDuration)
                {
                    attackTimer = 0f;
                    ChangeState(State.Chase);  // After regular melee, return to chase
                }
                break;

            case AttackType.Ranged:
                if (stateTimer >= rangedAttackDuration)
                {
                    attackTimer = 0f;
                    ChangeState(State.Chase);  // After ranged attack, return to chase
                }
                break;

            case AttackType.Spin:  // Handle spinning attack
                if (stateTimer >= spinChargeDuration)
                {
                    attackTimer = 0f;
                    ChangeState(State.Spinning);  // Transition to spinning state
                }
                break;
        }
    }


    private void ChooseAttack()
    {
        stateTimer = 0f;
        float distance = Vector3.Distance(centerTransform.position, player.position);
        if (distance <= meleeRange)
        {
            // Regular melee attack
            // currentAttack = AttackType.Melee;
            // PlayAnimation(MeleeAttackHash);
            // Randomly decide between Melee and Spin attack
            int randomAttack = UnityEngine.Random.Range(0, 2); // 0 or 1 for random choice
            if (randomAttack == 0)
            {
                currentAttack = AttackType.Melee;
                PlayAnimation(MeleeAttackHash);  // Regular melee attack
            }
            else
            {
                currentAttack = AttackType.Spin;  // Spinning attack // Call Spinning State
            }

            // currentAttack = AttackType.Spin;  // Spinning attack // Call Spinning State

        }
        else
        {
            currentAttack = AttackType.Ranged;
            PlayAnimation(RangeAttackHash);
        }
    }

    private float cooldownTimer = 0f; // Accumulates time for cooldown
    public float damageCooldown = 0.1f; // Time interval between damage applications during the spin (adjust this value)
    [SerializeField] private Vector2 spinHitboxSize = new Vector2(5f, 3f);
    [SerializeField] private bool showSpinHitbox = true;
    [SerializeField] private float spinDuration = 3f;
    private void SpinningState()
    {
        // --- CHARGE PHASE (Buff Animation) ---
        if (stateTimer <= spinChargeDuration)
        {
            // Play the buff animation while "charging"
            PlayAnimation(BuffHash);
            return;
        }

        // --- SPIN DAMAGE PHASE ---
        PlayAnimation(SpinningChargeAttackHash); // The actual spin attack animation

        // Tick damage based on cooldown
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            ApplySpinDamage();            // Apply damage to Hurtbox
            cooldownTimer = damageCooldown; // Reset cooldown
        }

        // --- END SPIN ---
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
                    stats.TakeDamage(1, centerTransform.position);
            }
        }
    }

    private void ChaseState()
    {
        if (currentState == State.Attack || isTurning) return;

        if (player == null)
        {
            ChangeState(State.Patrol);
            return;
        }

        // Vector3 characterOrigin = centerTransform.position + Vector3.right * (facingRight ? 2 : -2);
        // float distanceToRealPlayerOrigin = Vector3.Distance(characterOrigin, player.position);

        float distanceToPlayer = Vector3.Distance(centerTransform.position, player.position);

        // Player is close but attack is on cooldown → hold position
        if (distanceToPlayer <= meleeRange && attackTimer < attackCooldown)
        {
            // Only turn if facing wrong direction
            bool shouldFaceRight = player.position.x > transform.position.x;
            if (facingRight != shouldFaceRight)
            {
                wantsToFaceRight = shouldFaceRight;
                ChangeState(State.Turn);
            }
            return;
        }

        if (distanceToPlayer > detectionRange * 1.5f)
        {
            ChangeState(State.Patrol);
            return;
        }

        if (attackTimer >= attackCooldown)
        {
            if (distanceToPlayer <= meleeRange || distanceToPlayer <= rangedRange)
            {
                ChangeState(State.Attack);
                return;
            }
        }

        // Move towards player
        Vector3 direction = (player.position - centerTransform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Face player if needed
        bool playerOnRight = direction.x > 0;
        if (!isTurning && facingRight != playerOnRight)
        {
            wantsToFaceRight = playerOnRight;
            ChangeState(State.Turn);
        }
    }

    private void DeathState()
    {
        AncientBossHUD.SetActive(false);
        OnDeathStarted?.Invoke();
        isDead = true;
    }
    #endregion

    #region STATE MANAGEMENT
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
            case State.Dormant: PlayAnimation(SleepHash); break;
            case State.Wake: PlayAnimation(WakeHash); break;
            case State.Idle: PlayAnimation(IdleHash); break;
            case State.Patrol: PlayAnimation(RunHash); break;
            case State.Turn: PlayAnimation(TurnLeftHash); break;
            case State.Chase: PlayAnimation(RunHash); break;
            case State.Attack:
                {
                    ChooseAttack(); break;
                }
            case State.Death:
                PlayAnimation(DeathHash);
                isDead = true;
                // Notify LevelManager
                UnityEngine.Object.FindFirstObjectByType<LevelManager>()?.OnEnemyDefeated(); // convert these to an even call?
                break;
        }
    }

    private void OnStateExit(State state) { }
    #endregion

    #region COMBAT
    public void WakeUp()
    {
        if (currentState != State.Dormant)
            return; // Already awake or dead

        ChangeState(State.Wake);
    }

    private bool PlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector3.Distance(centerTransform.position - Vector3.left * 2, player.position) <= range;
    }
    public GameObject rockSpikePrefab;
    public float spikeSpacing = 2f;   // Distance between spikes
    public float delayBetweenSpikes = 0.3f;

    public void TriggerSpikeBarrage()
    {
        StartCoroutine(SpikeBarrage());
    }
    public IEnumerator SpikeBarrage()
    {
        float direction = facingRight ? 1f : -1f;
        Vector3 startPos = rockSpikeStartTransform.position;

        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = startPos + Vector3.right * direction * spikeSpacing * i;
            StartCoroutine(SpawnSpike(rockSpikePrefab, pos)); // Spawn independently
            yield return new WaitForSeconds(delayBetweenSpikes); // Stagger spawn timing
        }
    }

    IEnumerator SpawnSpike(GameObject spikePrefab, Vector3 position)
    {
        GameObject spike = Instantiate(spikePrefab, position, Quaternion.identity);

        RockSpike spikeComponent = spike.GetComponent<RockSpike>();
        if (spikeComponent != null)
        {
            spikeComponent.SetFacing(facingRight);
            spikeComponent.Emerge(); // play emerge animation
        }

        StartScreenshakeForAttacking();

        // Stay active for fixed duration (matches your attack window)
        yield return new WaitForSeconds(1.0f); // spike stays emerged

        if (spikeComponent != null)
        {
            spikeComponent.Retract(); // play retract animation
        }

        yield return new WaitForSeconds(0.1f); // retract animation duration
        Destroy(spike);
    }
    #endregion

    #region Hit Feedback Coroutines
    private IEnumerator ShakeSprite()
    {
        if (spriteRenderer == null) yield break;
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

    #endregion

    #region ANIMATION
    private void PlayAnimation(int animHash)
    {
        if (animator != null)
            animator.CrossFade(animHash, 0, 0);
    }
    #endregion

    #region FLIPPING
    private void Flip()
    {
        facingRight = !facingRight;
        ApplyFlip();
    }

    private void ApplyFlip()
    {
        if (visual != null)
        {
            Vector3 scale = visual.localScale;
            scale.x = facingRight ? 1f : -1f;
            visual.localScale = scale;

            Vector3 pos = visual.localPosition;
            pos.x = facingRight ? 0f : flipOffsetX;
            visual.localPosition = pos;
        }
    }
    #endregion

    #region HEALTH/ TAKING DAMAGE
    public void TakeDamage(int damageAmount, Vector2 attackerPosition, bool doScreenShake = true)
    {
        if (currentState == State.Dormant)
            return;

        if (isDead) return;
        // Debug.Log($"Ancient Boss took {damageAmount} damage.");
        // Sprite shake on hit
        // if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        // shakeCoroutine = StartCoroutine(ShakeSprite());
        // Sprite flash on hit
        onHitFlashVFX.PlayOnDamageVfx();

        if (AncientBossHUD != null && !AncientBossHUD.activeSelf)
            AncientBossHUD.SetActive(true);

        currentHealth -= damageAmount;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
            ChangeState(State.Death);

        StartScreenshakeFromGettingAttacked(attackerPosition);
    }
    #endregion

    #region SCREENSHAKE

    public void StartScreenshakeFromGettingAttacked(Vector2 attackerPosition)
    {
        if (impulseSource != null)
        {
            Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;
            ScreenShakeManager.Instance.ScreenShake(direction, impulseSource);
            Debug.Log("Ancient Boss Screen Shake From Getting Attacked");
        }
    }

    public void StartScreenshakeForAttacking()
    {
        if (impulseSource != null)
        {
            ScreenShakeManager.Instance.ScreenShake(impulseSource);
            Debug.Log("Ancient Boss Screen Shake For Attacking");
        }
    }
    #endregion

    #region GIZMOS
    private void OnDrawGizmosSelected()
    {
        // Vector3 center = transform.position + Vector3.right * (facingRight ? -2 : 2);
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

        if (!showSpinHitbox || centerTransform == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            centerTransform.position,
            spinHitboxSize
        );
    }
    #endregion
}
