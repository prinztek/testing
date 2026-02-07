using UnityEngine;

public class BarrelBomberEnemy : MonoBehaviour
{
    // ========================================
    // STATES
    // ========================================
    public enum State
    {
        Idle,           // Standing still, will return to patrol
        Patrol,
        Attack,         // Aim and throw bomb
        Recovery,       // After throwing
        Stunned,        // Hit by stun effect
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
    [SerializeField] private Transform throwPoint;
    [SerializeField] public BombProjectile bombPrefab;

    // ========================================
    // STATS
    // ========================================
    [Header("Stats")]
    [SerializeField] private int maxHealth = 25;
    [SerializeField] private int explosionDamage = 10;
    private int currentHealth;
    private bool isDead;

    // ========================================
    // PATROL
    // ========================================
    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 1.5f;

    // ========================================
    // COMBAT RANGES
    // ========================================
    [Header("Combat Ranges")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float minThrowRange = 3f;      // Don't throw if too close
    [SerializeField] private float maxThrowRange = 8f;

    // ========================================
    // COMBAT TIMING
    // ========================================
    [Header("Combat Timing")]
    [SerializeField] private float idleDuration = 1.5f;      // Time to pause at walls/ledges
    [SerializeField] private float attackDuration = 1.2f;    // Time for throw animation
    [SerializeField] private float recoveryTime = 1.5f;
    [SerializeField] private float stunDuration = 2.0f;

    // ========================================
    // BOMB SETTINGS
    // ========================================
    [Header("Bomb Settings")]
    [SerializeField] private float arcHeight = 3f;           // Height of the throw arc
    [SerializeField] private float explosionRadius = 2f;

    // ========================================
    // ANIMATIONS
    // ========================================
    private static readonly int IdleHash = Animator.StringToHash("idle");
    private static readonly int MoveHash = Animator.StringToHash("run");
    private static readonly int AttackHash = Animator.StringToHash("attack");
    private static readonly int DeathHash = Animator.StringToHash("dead");

    // ========================================
    // STATE DATA
    // ========================================
    private State currentState;
    private float stateTimer;
    private bool facingRight = true;
    private Transform player;
    private Vector3 targetThrowPosition;  // Where player was when we started attack
    private bool shouldFlipAfterIdle = false;

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
        ChangeState(State.Patrol);
    }

    private void Update()
    {
        if (isDead) return;

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                // Check for player during idle first
                if (PlayerInRange())
                {
                    DecideNextAction();
                }
                else
                {
                    Idle();  // Only process idle logic if no player
                }
                break;

            case State.Patrol:
                Patrol();

                // Check for player
                if (PlayerInRange())
                {
                    DecideNextAction();
                }
                break;

            case State.Attack:
                FacePlayer();

                if (stateTimer >= attackDuration)
                {
                    ChangeState(State.Recovery);
                }
                break;

            case State.Recovery:
                if (stateTimer >= recoveryTime)
                {
                    DecideNextAction();
                }
                break;

            case State.Stunned:
                // Just wait out the stun
                if (enemyStats.IsStunned() == false)
                {
                    DecideNextAction();
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
                break;

            case State.Patrol:
                PlayAnimation(MoveHash);
                break;

            case State.Attack:
                rb.linearVelocity = Vector2.zero;
                FacePlayer();
                // Store player's current position as throw target
                if (player != null)
                {
                    targetThrowPosition = player.position;
                }
                PlayAnimation(AttackHash);
                break;

            case State.Recovery:
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
    private void Idle()
    {
        // Just wait, then flip if needed and return to patrol
        if (stateTimer >= idleDuration)
        {
            if (shouldFlipAfterIdle)
            {
                FaceDirection(facingRight ? -1 : 1);
                shouldFlipAfterIdle = false;
            }
            ChangeState(State.Patrol);
        }
    }

    private void Patrol()
    {
        if (!enemyStats.Grounded())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        int dir = facingRight ? 1 : -1;

        // Check for walls or ledges
        bool noGroundAhead = !enemyStats.HasGroundAhead(dir);

        if (noGroundAhead)
        {
            shouldFlipAfterIdle = true;
            ChangeState(State.Idle);
            return;
        }

        rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);
    }

    private void DecideNextAction()
    {
        if (player == null || !PlayerInRange())
        {
            ChangeState(State.Patrol);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // Only attack if player is in good range (not too close, not too far)
        // if (distance >= minThrowRange && distance <= maxThrowRange)
        // attack regardless of distance
        if (distance <= maxThrowRange)
        {
            ChangeState(State.Attack);
        }
        else
        {
            // Move towards or away to get in range
            ChangeState(State.Patrol);
        }
    }

    // ========================================
    // RANGE CHECKS
    // ========================================
    private bool PlayerInRange()
    {
        if (player == null) return false;
        float distance = Vector2.Distance(transform.position, player.position);
        return distance <= detectionRange;
    }

    // ========================================
    // THROWING (Called by Animation Event)
    // ========================================
    public void ThrowBomb()
    {
        if (currentState != State.Attack) return;
        if (bombPrefab == null) return;

        // Spawn bomb at throw point
        BombProjectile bomb = Instantiate(bombPrefab, throwPoint.position, Quaternion.identity);

        // Set bomb properties
        bomb.SetDamage(explosionDamage, explosionRadius, gameObject);

        // Setup calculated launch to hit player's last position
        bomb.SetupLaunch(throwPoint.position, targetThrowPosition, arcHeight);
        bomb.Launch(calculateLaunch: true);  // Use calculated arc to hit target
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
        return explosionDamage;
    }

    // ========================================
    // STATUS EFFECTS
    // ========================================
    public void ApplyStun(float duration)
    {
        if (isDead) return;

        stunDuration = duration;
        ChangeState(State.Stunned);
    }

    public bool IsStunned()
    {
        return currentState == State.Stunned;
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

        // Max throw range (CYAN)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(pos, maxThrowRange);

        // Min throw range (RED)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, minThrowRange);

        // Throw point
        if (throwPoint != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(throwPoint.position, 0.1f);

            if (Application.isPlaying && player != null)
            {
                // Show calculated arc to player
                Gizmos.color = Color.green;
                DrawCalculatedArc(throwPoint.position, player.position, arcHeight);
            }
        }

        // Ground check
        if (enemyStats != null && enemyStats.groundCheckPoint != null)
        {
            int dir = facingRight ? 1 : -1;
            Vector3 origin = enemyStats.groundCheckPoint.position +
                            Vector3.right * dir * enemyStats.groundCheckX;

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(origin, origin + Vector3.down * enemyStats.groundCheckY);
        }
    }

    private void DrawCalculatedArc(Vector3 start, Vector3 end, float height)
    {
        int steps = 20;
        Vector3 previousPos = start;

        // Calculate the time and velocity for the arc
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float displacementY = end.y - start.y;
        float time = Mathf.Sqrt(-2 * height / -gravity) + Mathf.Sqrt(2 * (displacementY - height) / -gravity);

        for (int i = 1; i <= steps; i++)
        {
            float t = (i / (float)steps) * time;

            // Calculate position at time t using kinematic equations
            float x = Mathf.Lerp(start.x, end.x, i / (float)steps);
            float y = start.y + (Mathf.Sqrt(-2 * -gravity * height) * t) + (0.5f * -gravity * t * t);

            Vector3 pos = new Vector3(x, y, start.z);
            Gizmos.DrawLine(previousPos, pos);
            previousPos = pos;
        }
    }
}