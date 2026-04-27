using UnityEngine;

public class MeleeEnemy : MonoBehaviour
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
        Hurt,           // the player gets hit (pushed back a bit or stop moving forward)
        Death,
        Edge,
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

    // ========================================
    // STATS
    // ========================================
    [Header("Stats")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int damage = 15;
    private float knockbackForce = 10f; // how far the enemy is knocked back
    private int currentHealth;
    private bool isDead;

    // ========================================
    // HURT
    // ========================================
    [Header("Hurt")]
    [SerializeField] private float hurtDuration = 0.333f;
    // ========================================
    // PATROL & IDLE
    // ========================================
    [Header("Patrol & Idle")]
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float idleDuration = 2.5f;
    [SerializeField] private float patrolDuration = 4f;
    private float patrolTimer;

    // ========================================
    // COMBAT
    // ========================================
    [Header("Combat")]
    [SerializeField] private float detectionRange = 6f;     // How far skeleton can see
    [SerializeField] private float detectionHeightTolerance = 1.2f; // vertical window
    [SerializeField] private float chaseSpeed;         // Speed when chasing player
    [SerializeField] private float attackRange = 1.5f;      // Melee attack range
    [SerializeField] private float attackRadius = 1.2f;     // Attack hitbox radius

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
    [SerializeField] private float giveUpDistance = 12f;    // Stop chasing if player this far
    [SerializeField] private bool canFallOffLedges = false; // Whether skeleton walks off edges

    [Header("Attack Nudge")]
    [SerializeField] private float attackNudgeForce = 0f;
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
    // HURT DATA
    // ========================================
    private Vector2 pendingHitDir;
    private float pendingForceX;
    private float pendingForceY;

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
        // Start Idle
        ChangeState(State.Idle);
    }

    private void HandleHurt(Vector2 dir, float fx, float fy)
    {
        if (currentState == State.Death) return;

        pendingHitDir = dir;
        pendingForceX = fx;
        pendingForceY = fy;

        ChangeState(State.Hurt);
    }

    private void Update()
    {
        if (isDead) return;

        // Check for stun before anything else
        if (enemyStats.activeStatus != null && !enemyStats.canMove)
        {
            if (currentState != State.Stunned)
                ChangeState(State.Stunned);
        }

        if (currentState == State.Hurt)
        {
            stateTimer += Time.deltaTime;

            if (stateTimer >= hurtDuration)
            {
                enemyStats.isHurt = false;

                if (PlayerDetected()) ChangeState(State.Chase);
                else ChangeState(State.Patrol);
            }

            return; // stop other logic
        }

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
                else if (PlayerTooFar())
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

            case State.Hurt:
                if (stateTimer >= hurtDuration)
                {
                    enemyStats.isHurt = false;

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
            case State.Edge:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

                int dir = facingRight ? 1 : -1;

                FacePlayer(); // keep looking at player initially

                if (!PlayerDetected())
                {
                    ChangeState(State.Patrol);
                    return;
                }

                if (InAttackRange())
                {
                    ChangeState(State.Attack);
                    return;
                }

                if (stateTimer > 1.0f)
                {
                    Debug.Log("Flipping at edge");
                    FaceDirection(-dir);   // flip away from edge
                    ChangeState(State.Patrol);
                }
                break;

            case State.Stunned:
                if (enemyStats.IsStunned() == false)
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
                rb.linearVelocity = Vector2.zero;
                FacePlayer();
                PlayAnimation(AttackHash);
                // attackNudgeApplied = false;
                break;

            case State.Recovery:
                rb.linearVelocity = Vector2.zero;
                PlayAnimation(IdleHash);
                break;

            case State.Hurt:
                PlayAnimation(IdleHash);

                // Vector2 hitDir = enemyStats.GetLastHitDirection().normalized;

                ApplyKnockback(pendingHitDir, pendingForceX, pendingForceY);
                break;
            case State.Edge:
                rb.linearVelocity = Vector2.zero;
                PlayAnimation(IdleHash);
                break;

            case State.Stunned:
                rb.linearVelocity = Vector2.zero;
                PlayAnimation(IdleHash);
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

        if (enemyStats.canMove == false) return;

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

        rb.linearVelocity = new Vector2(dir * enemyStats.GetMoveSpeed(), rb.linearVelocity.y);
    }

    private void ChasePlayer()
    {
        if (player == null) return;
        if (enemyStats.canMove == false) return;

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
            // rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            ChangeState(State.Edge);
            return;
        }

        FaceDirection(dir);
        rb.linearVelocity = new Vector2(dir * enemyStats.GetMoveSpeed(), rb.linearVelocity.y);
    }

    // ========================================
    // DETECTION & RANGE
    // ========================================
    private bool PlayerDetected()
    {
        if (player == null) return false;
        float dx = Mathf.Abs(player.position.x - transform.position.x);
        float dy = Mathf.Abs(player.position.y - transform.position.y);
        return dx <= detectionRange && dy <= detectionHeightTolerance;
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

        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            HandlePlayerSpawned(GameManager.Instance.CurrentPlayer);
        }

        enemyStats.OnHurt += HandleHurt;

    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
        enemyStats.OnHurt -= HandleHurt;
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
        Vector3 center = transform.position;
        Vector3 size = new Vector3(detectionRange * 2f, detectionHeightTolerance * 2f, 0f);
        Gizmos.DrawWireCube(center, size);

        // Attack range (RED)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, attackRange);

        Gizmos.color = Color.purple;
        Vector3 giveUpDistanceSize = new Vector3(giveUpDistance * 2f, detectionHeightTolerance * 2f, 0f);
        Gizmos.DrawWireCube(center, giveUpDistanceSize);

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

    private void ApplyKnockback(Vector2 hitDir, float forceX, float forceY)
    {
        // Debug.Log($"Applying knockback with direction {hitDir}, forceX {forceX}, forceY {forceY}");
        // Reset current velocity so knockback is consistent
        rb.linearVelocity = Vector2.zero;

        Vector2 force = new Vector2(hitDir.x * forceX, forceY);

        rb.AddForce(force, ForceMode2D.Impulse);
    }
}