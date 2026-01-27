using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class AncientBoss : MonoBehaviour
{
    // ========================================
    // STATES
    // ========================================
    public enum State
    {
        Dormant,
        Wake,
        Idle,
        Approach, // like a run or closing gap between the player and the ancient boss
        Attack,
        Recovery,
        Death
    }

    public enum BossAttackType
    {
        Stomp,      // ground spikes
        Laser,      // mid-range punish
        Spin        // close-range denial
    }

    // ========================================
    // REFERENCES
    // ========================================
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform centerTransform;  // sprite center 
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform rockSpikeStartTransform;
    [SerializeField] private GameObject rockSpikePrefab;
    [SerializeField] private GameObject bossHUD;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    // ========================================
    // TIMING
    // ========================================
    [Header("Timing")]
    [SerializeField] private float wakeTime = 1.2f;
    [SerializeField] private float idleBeforeAttack = 0.8f;
    [SerializeField] private float attackDuration = 1.2f; // matches melee animation
    [SerializeField] private float recoveryAfterAttack = 1.0f;

    // ========================================
    // SPIKE ATTACK
    // ========================================
    [Header("Spike Barrage")]
    [SerializeField] private float spikeSpacing = 2f;
    [SerializeField] private float delayBetweenSpikes = 0.3f;
    [SerializeField] private int spikeCount = 3;

    // ========================================
    // STATS
    // ========================================
    [Header("Stats")]
    [SerializeField] public int maxHealth = 200;
    public int currentHealth;
    private bool isDead;

    [SerializeField] private int damage;

    [Header("Approach")]
    [SerializeField] private float approachSpeed = 3.5f;
    [SerializeField] private float desiredAttackRange = 1.4f;
    [SerializeField] private float maxApproachTime = 1.2f;

    [SerializeField] private float turnPauseTime = 0.15f;
    private float lastTurnTime;



    // ========================================
    // ANIMATIONS
    // ========================================
    private static readonly int SleepHash = Animator.StringToHash("sleep");
    private static readonly int WakeHash = Animator.StringToHash("wake");
    private static readonly int IdleHash = Animator.StringToHash("idle");
    private static readonly int RunHash = Animator.StringToHash("run");
    private static readonly int MeleeAttackHash = Animator.StringToHash("meleeAttack");
    private static readonly int LaserAttackHash = Animator.StringToHash("rangeAttack");
    private static readonly int SpinAttackHash = Animator.StringToHash("spinCharge");
    private static readonly int DeathHash = Animator.StringToHash("dead");

    // ========================================
    // STATE DATA
    // ========================================
    private State currentState;
    public BossAttackType currentAttack;
    public int attackDirection { get; private set; } // -1 left, +1 right
    [SerializeField] private float repositionSpeed = 3f;
    private float stateTimer;
    private bool facingRight = true;
    private bool isAttacking;

    [Header("Attack Ranges")]
    [SerializeField] private float closeRange = 1.2f;   // Spin
    [SerializeField] private float midRange = 2.0f;   // Stomp
    [SerializeField] private float farRange = 4.0f;   // Laser


    // ========================================
    // UNITY
    // ========================================
    private Transform player;
    private CharacterStats playerStats;
    private Coroutine currentAttackCoroutine;

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        player = playerObj.transform;
        playerStats = player.GetComponent<CharacterStats>();
    }

    private void Awake()
    {
        animator ??= GetComponentInChildren<Animator>();
        spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
        visual ??= spriteRenderer.transform;
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        ChangeState(State.Dormant);
    }

    private void Update()
    {
        if (isDead) return;

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Wake:
                if (stateTimer >= wakeTime)
                    ChangeState(State.Idle);
                break;

            case State.Idle:
                if (stateTimer >= idleBeforeAttack)
                {
                    SelectAttack();

                    if (NeedsToCloseGap())
                        ChangeState(State.Approach);
                    else
                        ChangeState(State.Attack);
                }
                break;

            case State.Approach:
                MoveTowardPlayer(); // move to fixedUpdate?

                if (InAttackRange() || stateTimer >= maxApproachTime)
                    ChangeState(State.Attack);
                break;

            case State.Attack:
                // Wait for animation / events to handle hitboxes
                if (stateTimer >= attackDuration)
                    ChangeState(State.Recovery);
                break;

            case State.Recovery:
                if (stateTimer >= recoveryAfterAttack)
                    ChangeState(State.Idle);
                break;
        }
    }

    // private void FixedUpdate()
    // {
    //     if (currentState == State.Approach)
    //     {
    //         MoveTowardPlayer();
    //     }
    // }

    // ========================================
    // STATE MACHINE
    // ========================================
    private void ChangeState(State newState)
    {
        currentState = newState;
        stateTimer = 0f;

        switch (newState)
        {
            case State.Dormant:
                PlayAnimation(SleepHash);
                break;

            case State.Wake:
                PlayAnimation(WakeHash);
                break;

            case State.Idle:
                rb.linearVelocity = Vector2.zero;
                PlayAnimation(IdleHash);
                break;

            case State.Approach:
                PlayAnimation(RunHash);
                break;

            case State.Attack:
                isAttacking = true;
                rb.linearVelocity = Vector2.zero;
                AttackHandler();
                // wrap attack because there are multiple attacks
                // we can't just play the attack animation here 
                break;

            case State.Recovery:
                isAttacking = false;
                PlayAnimation(IdleHash);
                break;

            case State.Death:
                PlayAnimation(DeathHash);
                isDead = true;
                bossHUD?.SetActive(false);
                break;
        }
    }

    // ========================================
    // DIRECTION & TARGETING
    // ========================================

    private bool NeedsToCloseGap()
    {
        if (player == null) return false;

        float desiredRange = GetDesiredRangeForCurrentAttack();
        float distance = Mathf.Abs(player.position.x - centerTransform.position.x);

        return distance > desiredRange + 0.2f;
    }

    private bool InAttackRange()
    {
        if (player == null) return true;

        float desiredRange = GetDesiredRangeForCurrentAttack();
        float distance = Mathf.Abs(player.position.x - centerTransform.position.x);

        return distance <= desiredRange;
    }


    private void MoveTowardPlayer()
    {
        if (player == null) return;

        int desiredDir = player.position.x < centerTransform.position.x ? -1 : 1;

        // If not facing the player yet → turn first, do not move
        if ((desiredDir > 0 && !facingRight) || (desiredDir < 0 && facingRight))
        {
            FaceDirection(desiredDir);
            lastTurnTime = Time.time;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Time.time - lastTurnTime < turnPauseTime)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        // Move ONLY in the facing direction
        float moveDir = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(moveDir * approachSpeed, rb.linearVelocity.y);
    }


    private void LockAttackDirection()
    {
        if (player == null) return;

        attackDirection = player.position.x < centerTransform.position.x ? -1 : 1;
        FaceDirection(attackDirection);
    }

    private void FaceDirection(int dir)
    {
        if ((dir > 0 && facingRight) || (dir < 0 && !facingRight)) return;

        facingRight = dir > 0;
        Vector3 scale = visual.localScale;
        scale.x = facingRight ? 1f : -1f;
        visual.localScale = scale;

        Vector3 pos = visual.localPosition;
        pos.x = facingRight ? 0f : -4.4f;
        visual.localPosition = pos;
    }

    // ========================================
    // ATTACK SELECTION
    // ========================================
    public void SelectAttack()
    {
        if (player == null) return;

        float distance = Mathf.Abs(player.position.x - centerTransform.position.x);

        if (distance <= closeRange)
        {
            currentAttack = BossAttackType.Spin;
        }
        else if (distance <= midRange)
        {
            currentAttack = BossAttackType.Stomp;
        }
        else
        {
            currentAttack = BossAttackType.Laser;
        }
    }

    private void PlayAttackAnimation()
    {
        switch (currentAttack)
        {
            case BossAttackType.Stomp:
                PlayAnimation(MeleeAttackHash);
                break;
            case BossAttackType.Laser:
                PlayAnimation(LaserAttackHash);
                break;
            case BossAttackType.Spin:
                PlayAnimation(SpinAttackHash);
                break;
        }
    }

    private void AttackHandler()
    {
        LockAttackDirection();
        PlayAttackAnimation();
    }

    // ========================================
    // ANIMATION EVENT ENTRY POINTS
    // ========================================
    public void TriggerSpikeBarrage()
    {
        if (!isAttacking || currentAttack != BossAttackType.Stomp) return;
        StartCoroutine(SpikeBarrageRoutine());
    }

    private IEnumerator SpikeBarrageRoutine()
    {
        Vector3 startPos = rockSpikeStartTransform.position;
        for (int i = 0; i < spikeCount; i++)
        {
            Vector3 pos = startPos + Vector3.right * attackDirection * spikeSpacing * i;
            SpawnSpike(pos);
            yield return new WaitForSeconds(delayBetweenSpikes);
        }
    }

    private void SpawnSpike(Vector3 position)
    {
        GameObject spike = Instantiate(rockSpikePrefab, position, Quaternion.identity);
        RockSpike spikeComponent = spike.GetComponent<RockSpike>();
        if (spikeComponent != null)
        {
            spikeComponent.SetFacing(facingRight);
            spikeComponent.Emerge();
        }

        if (impulseSource != null)
            ScreenShakeManager.Instance.ScreenShake(impulseSource);

        Destroy(spike, 1.2f);
    }

    // ========================================
    // PUBLIC METHODS
    // ========================================
    public void WakeUp()
    {
        if (currentState == State.Dormant)
            ChangeState(State.Wake);
    }

    public int GetDamage()
    {
        return damage;
    }

    // ========================================
    // HELPERS
    // ========================================
    private void PlayAnimation(int hash)
    {
        animator.CrossFade(hash, 0f, 0);
    }

    private float GetDesiredRangeForCurrentAttack()
    {
        switch (currentAttack)
        {
            case BossAttackType.Spin:
                return closeRange;

            case BossAttackType.Stomp:
                return midRange;

            case BossAttackType.Laser:
                return farRange;

            default:
                return midRange;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (centerTransform == null) return;

        Vector3 center = centerTransform.position;

        // Close range (Spin)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, closeRange);

        // Mid range (Stomp)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, midRange);

        // Far range (Laser)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, farRange);
    }

}
