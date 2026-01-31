using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkeletonArmoredSwordsmanEnemy : MonoBehaviour
{
    // ========================================
    // STATES
    // ========================================
    public enum State
    {
        Idle,
        Patrol,
        Detect,         // Spotted player, prepare to engage
        Chase,          // Running toward player
        Attack,         // Melee attack
        Recovery,       // After attack cooldown
        Stunned,        // Hit reaction (optional)
        Death
    }

    // ========================================
    // REFERENCES
    // ========================================
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform visual;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Transform attackPoint;        // Where to check for hits

    // ========================================
    // STATS
    // ========================================
    [Header("Stats")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int damage = 15;
    private int currentHealth;
    private bool isDead;

    // ========================================
    // PATROL & IDLE
    // ========================================
    [Header("Patrol & Idle")]
    [SerializeField] private float patrolSpeed = 1.2f;
    [SerializeField] private float idleDuration = 2.5f;
    [SerializeField] private float patrolDuration = 4f;
    private float patrolTimer;

    // ========================================
    // COMBAT
    // ========================================
    [Header("Combat")]
    [SerializeField] private float detectionRange = 6f;     // How far skeleton can see
    [SerializeField] private float chaseSpeed = 3f;         // Speed when chasing player
    [SerializeField] private float attackRange = 1.5f;      // Melee attack range
    [SerializeField] private float attackRadius = 1.2f;     // Attack hitbox radius
    [SerializeField] private LayerMask playerLayer;         // What counts as player

    // ========================================
    // TIMING
    // ========================================
    [Header("Timing")]
    [SerializeField] private float detectTime = 0.4f;       // Windup before chase
    [SerializeField] private float attackDuration = 0.8f;   // Attack animation length
    [SerializeField] private float recoveryTime = 1.0f;     // Cooldown after attack
    [SerializeField] private float stunDuration = 0.5f;     // Time stunned when hit

    // ========================================
    // BEHAVIOR
    // ========================================
    [Header("Behavior")]
    [SerializeField] private bool aggressive = true;        // If true, chases relentlessly
    [SerializeField] private float giveUpDistance = 12f;    // Stop chasing if player this far
    [SerializeField] private bool canFallOffLedges = false; // Whether skeleton walks off edges

    [Header("Attack Nudge")]
    [SerializeField] private float attackNudgeForce = 2.5f;
    private bool attackNudgeApplied;

    // ========================================
    // ANIMATIONS
    // ========================================
    private static readonly int IdleHash = Animator.StringToHash("idle");
    private static readonly int MoveHash = Animator.StringToHash("run");
    private static readonly int AttackHash = Animator.StringToHash("meleeAttack");
    private static readonly int DeathHash = Animator.StringToHash("dead");
    private static readonly int HurtHash = Animator.StringToHash("enemyhurt");  // Optional

    // ========================================
    // STATE DATA
    // ========================================
    private State currentState;
    private float stateTimer;
    private bool facingRight = true;

    private Transform player;

    // ========================================
    // UNITY
    // ========================================
    private void Awake()
    {
        enemyStats ??= GetComponent<EnemyStats>();
        animator ??= GetComponentInChildren<Animator>();
        spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
        visual ??= spriteRenderer.transform;
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        // Start randomly between Idle and Patrol
        ChangeState(Random.value > 0.5f ? State.Idle : State.Patrol);
    }

    private void Update()
    {
        if (isDead) return;

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

                if (PlayerDetected())
                {
                    ChangeState(State.Detect);
                }
                else if (stateTimer >= idleDuration)
                {
                    ChangeState(State.Patrol);
                }
                break;

            case State.Patrol:
                Patrol();

                if (PlayerDetected())
                {
                    ChangeState(State.Detect);
                }
                break;

            case State.Detect:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                FacePlayer();

                if (stateTimer >= detectTime)
                {
                    ChangeState(State.Chase);
                }
                break;

            case State.Chase:
                ChasePlayer();

                if (InAttackRange())
                {
                    ChangeState(State.Attack);
                }
                else if (!aggressive && PlayerTooFar())
                {
                    // Give up chase if not aggressive
                    ChangeState(State.Patrol);
                }
                break;

            case State.Attack:
                // Attack animation plays, damage dealt via animation event

                if (stateTimer >= attackDuration)
                {
                    ChangeState(State.Recovery);
                }
                break;

            case State.Recovery:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

                if (stateTimer >= recoveryTime)
                {
                    // Decide what to do after recovery
                    if (InAttackRange())
                    {
                        ChangeState(State.Attack);
                    }
                    else if (PlayerDetected())
                    {
                        ChangeState(State.Chase);
                    }
                    else
                    {
                        ChangeState(State.Patrol);
                    }
                }
                break;

            case State.Stunned:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

                if (stateTimer >= stunDuration)
                {
                    // Return to combat after stun
                    if (PlayerDetected())
                    {
                        ChangeState(State.Chase);
                    }
                    else
                    {
                        ChangeState(State.Patrol);
                    }
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        if (currentState == State.Attack && !attackNudgeApplied)
        {
            int dir = facingRight == true ? 1 : -1;
            rb.AddForce(Vector2.right * dir * attackNudgeForce, ForceMode2D.Impulse);
            attackNudgeApplied = true;
        }
    }

    // ========================================
    // STATE MACHINE
    // ========================================
    private void ChangeState(State newState)
    {
        currentState = newState;
        stateTimer = 0f;

        switch (newState)
        {
            case State.Idle:
                rb.linearVelocity = Vector2.zero;
                PlayAnimation(IdleHash);
                patrolTimer = 0f;
                break;

            case State.Patrol:
                PlayAnimation(MoveHash);
                patrolTimer = 0f;
                break;

            case State.Detect:
                rb.linearVelocity = Vector2.zero;
                PlayAnimation(IdleHash);
                break;

            case State.Chase:
                PlayAnimation(MoveHash);
                break;

            case State.Attack:
                // rb.linearVelocity = Vector2.zero;
                FacePlayer();
                PlayAnimation(AttackHash);
                attackNudgeApplied = false;
                break;

            case State.Recovery:
                rb.linearVelocity = Vector2.zero;
                PlayAnimation(IdleHash);
                break;

            case State.Stunned:
                rb.linearVelocity = Vector2.zero;
                // PlayAnimation(HurtHash);
                break;

            case State.Death:
                rb.linearVelocity = Vector2.zero;
                // PlayAnimation(DeathHash);
                isDead = true;
                break;
        }
    }

    // ========================================
    // BEHAVIOR
    // ========================================
    private void Patrol()
    {
        if (!enemyStats.Grounded())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // Track patrol time
        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolDuration)
        {
            ChangeState(State.Idle);
            return;
        }

        int dir = facingRight ? 1 : -1;

        // Edge detection
        if (!canFallOffLedges && !enemyStats.HasGroundAhead(dir))
        {
            FaceDirection(-dir);
            return;
        }

        rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        if (!enemyStats.Grounded())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        int dir = player.position.x < transform.position.x ? -1 : 1;

        // Check for ground ahead (unless can fall off ledges)
        if (!canFallOffLedges && !enemyStats.HasGroundAhead(dir))
        {
            // Can't continue chase, stop at edge
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        FaceDirection(dir);
        rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);
    }

    // ========================================
    // DETECTION & RANGE
    // ========================================
    private bool PlayerDetected()
    {
        if (player == null) return false;

        float distance = Mathf.Abs(player.position.x - transform.position.x);
        return distance <= detectionRange;
    }

    private bool InAttackRange()
    {
        if (player == null) return false;

        float distance = Mathf.Abs(player.position.x - transform.position.x);
        return distance <= attackRange;
    }

    private bool PlayerTooFar()
    {
        if (player == null) return true;

        float distance = Mathf.Abs(player.position.x - transform.position.x);
        return distance > giveUpDistance;
    }

    // ========================================
    // COMBAT (Called by Animation Event)
    // ========================================
    public void OnAttackHit()
    {
        if (currentState != State.Attack) return;
        if (attackPoint == null) return;

        // Check for player in attack range
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);

        foreach (Collider2D hit in hits)
        {
            // Try to damage player
            CharacterStats playerStats = hit.GetComponent<CharacterStats>();
            if (playerStats != null)
            {
                Vector2 attackDirection = hit.transform.position - transform.position;
                playerStats.TakeDamage(damage, transform.position);

                Debug.Log($"[Skeleton] Hit player for {damage} damage!");
            }
        }
    }

    // ========================================
    // DAMAGE HANDLING
    // ========================================
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            ChangeState(State.Death);
        }
        else
        {
            // Go into stunned state when hit
            ChangeState(State.Stunned);
        }
    }

    // ========================================
    // DIRECTION
    // ========================================
    private void FacePlayer()
    {
        if (player == null) return;

        int dir = player.position.x < transform.position.x ? -1 : 1;
        FaceDirection(dir);
    }

    private void FaceDirection(int dir)
    {
        if ((dir > 0 && facingRight) || (dir < 0 && !facingRight)) return;

        facingRight = dir > 0;

        Vector3 scale = visual.localScale;
        scale.x = facingRight ? 1f : -1f;
        visual.localScale = scale;
    }

    // ========================================
    // HELPERS
    // ========================================
    private void PlayAnimation(int hash)
    {
        animator.CrossFade(hash, 0f, 0);
    }

    public int GetDamage()
    {
        return damage;
    }

    // ========================================
    // PLAYER HOOK
    // ========================================
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
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;

        // Detection range (YELLOW)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, detectionRange);

        // Attack range (RED)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, attackRange);

        // Attack hitbox (MAGENTA)
        if (attackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }

        // Give up distance (CYAN)
        if (!aggressive)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(pos, giveUpDistance);
        }

        // Ground check
        if (enemyStats != null && enemyStats.groundCheckPoint != null)
        {
            int dir = facingRight ? 1 : -1;
            Vector3 origin = enemyStats.groundCheckPoint.position +
                            Vector3.right * dir * enemyStats.groundCheckX;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, origin + Vector3.down * enemyStats.groundCheckY);
        }
    }
}