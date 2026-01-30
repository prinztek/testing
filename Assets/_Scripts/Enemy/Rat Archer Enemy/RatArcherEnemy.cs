using UnityEngine;

public class RatArcherEnemy : MonoBehaviour
{
    // ========================================
    // STATES
    // ========================================
    public enum State
    {
        Patrol,
        Detect,         // Spotted player, prepare to engage
        KeepDistance,   // Maintain optimal shooting range
        Retreat,        // Player too close, back away
        Attack,         // Aim and shoot (combined)
        Recovery,       // After shooting
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
    [SerializeField] private Transform shootPoint; // Where arrows spawn from
    [SerializeField] public EnemyProjectile projectilePrefab;
    public float projectileSpeed = 8f;

    // ========================================
    // STATS
    // ========================================
    [Header("Stats")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int damage = 5;
    private int currentHealth;
    private bool isDead;

    // ========================================
    // PATROL
    // ========================================
    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 1.0f;

    // ========================================
    // COMBAT RANGES
    // ========================================
    [Header("Combat Ranges")]
    [SerializeField] private float detectionRange = 8f;      // How far rat can see player
    [SerializeField] private float optimalShootRange = 5f;   // Preferred attack distance
    [SerializeField] private float minShootRange = 3f;       // Minimum comfortable distance
    [SerializeField] private float retreatRange = 2f;        // If player closer, retreat!
    [SerializeField] private float maxShootRange = 7f;       // Maximum shooting distance

    // ========================================
    // MOVEMENT
    // ========================================
    [Header("Movement")]
    [SerializeField] private float keepDistanceSpeed = 2f;   // Speed when repositioning
    [SerializeField] private float retreatSpeed = 3f;        // Speed when backing away

    // ========================================
    // COMBAT TIMING
    // ========================================
    [Header("Combat Timing")]
    [SerializeField] private float detectTime = 0.3f;        // Time spent in Detect state
    [SerializeField] private float attackDuration = 1.0f;    // Full attack animation time (aim + shoot)
    [SerializeField] private float recoveryTime = 0.8f;      // Cooldown after shooting
    [SerializeField] private float repositionTime = 1.5f;    // Max time spent repositioning

    // ========================================
    // BEHAVIOR SETTINGS
    // ========================================
    [Header("Behavior")]
    [SerializeField] private bool canRetreatOffLedges = false; // Whether rat can back off edges
    [SerializeField] private float rangeCheckInterval = 0.2f;   // How often to check distance
    private float rangeCheckTimer;

    // ========================================
    // ANIMATIONS
    // ========================================
    private static readonly int IdleHash = Animator.StringToHash("enemyidle");
    private static readonly int MoveHash = Animator.StringToHash("enemyrunning");
    private static readonly int AttackHash = Animator.StringToHash("enemyattack1");
    private static readonly int DeathHash = Animator.StringToHash("dead");

    // ========================================
    // STATE DATA
    // ========================================
    private State currentState;
    private float stateTimer;
    private bool facingRight = true;
    private float cachedPlayerDistance;

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
        ChangeState(State.Patrol);
    }

    private void Update()
    {
        if (isDead) return;

        stateTimer += Time.deltaTime;
        rangeCheckTimer += Time.deltaTime;

        // Update cached distance periodically
        if (rangeCheckTimer >= rangeCheckInterval && player != null)
        {
            rangeCheckTimer = 0f;
            cachedPlayerDistance = Mathf.Abs(player.position.x - transform.position.x);
        }

        switch (currentState)
        {
            case State.Patrol:
                Patrol();

                if (PlayerDetected())
                {
                    ChangeState(State.Detect);
                }
                break;

            case State.Detect:
                // Face player and prepare for combat
                FacePlayer();

                if (stateTimer >= detectTime)
                {
                    EvaluateCombatState();
                }
                break;

            case State.KeepDistance:
                KeepOptimalDistance();

                if (!PlayerInRange())
                {
                    ChangeState(State.Patrol);
                }
                else if (PlayerTooClose())
                {
                    ChangeState(State.Retreat);
                }
                else if (InOptimalShootRange())
                {
                    ChangeState(State.Attack);
                }
                else if (stateTimer >= repositionTime)
                {
                    // Been repositioning too long, just shoot anyway
                    ChangeState(State.Attack);
                }
                break;

            case State.Retreat:
                RetreatFromPlayer();

                if (!PlayerTooClose() && InOptimalShootRange())
                {
                    ChangeState(State.Attack);
                }
                else if (!PlayerInRange())
                {
                    ChangeState(State.Patrol);
                }
                break;

            case State.Attack:
                // Stay still, face player, animation plays aim + shoot
                FacePlayer();

                if (stateTimer >= attackDuration)
                {
                    ChangeState(State.Recovery);
                }
                else if (PlayerTooClose() && stateTimer < attackDuration * 0.3f)
                {
                    // Player rushed in early in animation, abort and retreat
                    // Only allow abort in first 30% of animation
                    ChangeState(State.Retreat);
                }
                break;

            case State.Recovery:
                if (stateTimer >= recoveryTime)
                {
                    EvaluateCombatState();
                }
                else if (PlayerTooClose())
                {
                    // Emergency retreat even during recovery
                    ChangeState(State.Retreat);
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
            case State.Patrol:
                PlayAnimation(MoveHash);
                break;

            case State.Detect:
                rb.linearVelocity = Vector2.zero;
                PlayAnimation(IdleHash);
                break;

            case State.KeepDistance:
                PlayAnimation(MoveHash);
                break;

            case State.Retreat:
                PlayAnimation(MoveHash);
                break;

            case State.Attack:
                rb.linearVelocity = Vector2.zero;
                FacePlayer();
                PlayAnimation(AttackHash);
                break;

            case State.Recovery:
                rb.linearVelocity = Vector2.zero;
                PlayAnimation(IdleHash);
                break;

            case State.Death:
                rb.linearVelocity = Vector2.zero;
                PlayAnimation(DeathHash);
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

        int dir = facingRight ? 1 : -1;

        // Edge detection
        if (!enemyStats.HasGroundAhead(dir))
        {
            FaceDirection(-dir);
            return;
        }

        rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);
    }

    private void KeepOptimalDistance()
    {
        if (player == null) return;

        if (!enemyStats.Grounded())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float distance = cachedPlayerDistance;
        int dirToPlayer = player.position.x < transform.position.x ? -1 : 1;

        // Too far - move closer
        if (distance > optimalShootRange)
        {
            if (!enemyStats.HasGroundAhead(dirToPlayer))
            {
                // Can't move closer due to pit, just attack from here
                ChangeState(State.Attack);
                return;
            }

            FaceDirection(dirToPlayer);
            rb.linearVelocity = new Vector2(dirToPlayer * keepDistanceSpeed, rb.linearVelocity.y);
        }
        // Too close - back away slightly
        else if (distance < minShootRange)
        {
            int retreatDir = -dirToPlayer;

            if (!canRetreatOffLedges && !enemyStats.HasGroundAhead(retreatDir))
            {
                // Cornered! Just shoot from here
                ChangeState(State.Attack);
                return;
            }

            FacePlayer(); // Face player while backing away
            rb.linearVelocity = new Vector2(retreatDir * keepDistanceSpeed, rb.linearVelocity.y);
        }
        // Just right - stop and prepare to shoot
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void RetreatFromPlayer()
    {
        if (player == null) return;

        if (!enemyStats.Grounded())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        int dirToPlayer = player.position.x < transform.position.x ? -1 : 1;
        int retreatDir = -dirToPlayer;

        // Check if can retreat
        if (!canRetreatOffLedges && !enemyStats.HasGroundAhead(retreatDir))
        {
            // Cornered! Stand ground and shoot
            rb.linearVelocity = Vector2.zero;
            ChangeState(State.Attack);
            return;
        }

        FacePlayer(); // Face player while retreating
        rb.linearVelocity = new Vector2(retreatDir * retreatSpeed, rb.linearVelocity.y);
    }

    private void EvaluateCombatState()
    {
        if (player == null)
        {
            ChangeState(State.Patrol);
            return;
        }

        if (!PlayerInRange())
        {
            ChangeState(State.Patrol);
        }
        else if (PlayerTooClose())
        {
            ChangeState(State.Retreat);
        }
        else if (InOptimalShootRange())
        {
            ChangeState(State.Attack);
        }
        else
        {
            ChangeState(State.KeepDistance);
        }
    }

    // ========================================
    // RANGE CHECKS
    // ========================================
    private bool PlayerDetected()
    {
        if (player == null) return false;

        float distance = Mathf.Abs(player.position.x - transform.position.x);
        return distance <= detectionRange;
    }

    private bool PlayerInRange()
    {
        if (player == null) return false;

        float distance = cachedPlayerDistance;
        return distance <= maxShootRange;
    }

    private bool PlayerTooClose()
    {
        if (player == null) return false;

        float distance = cachedPlayerDistance;
        return distance < retreatRange;
    }

    private bool InOptimalShootRange()
    {
        if (player == null) return false;

        float distance = cachedPlayerDistance;
        return distance >= minShootRange && distance <= optimalShootRange;
    }

    // ========================================
    // SHOOTING (Called by Animation Event)
    // ========================================
    public void OnShootArrow()
    {
        if (currentState != State.Attack) return;
        if (player == null) return;
        // Flip direction based on character facing
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        EnemyProjectile proj =
            Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        proj.SetDamage(damage, gameObject);
        proj.Launch(direction * projectileSpeed);

        Debug.Log("[RatArcher] Arrow shot! (Implement projectile spawning here)");
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

        // Max shoot range (CYAN)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(pos, maxShootRange);

        // Optimal shoot range (GREEN)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, optimalShootRange);
        Gizmos.DrawWireSphere(pos, minShootRange);

        // Retreat range (RED)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, retreatRange);

        // Ground check
        if (enemyStats != null && enemyStats.groundCheckPoint != null)
        {
            int dir = facingRight ? 1 : -1;
            Vector3 origin = enemyStats.groundCheckPoint.position +
                            Vector3.right * dir * enemyStats.groundCheckX;

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(origin, origin + Vector3.down * enemyStats.groundCheckY);
        }

        // Shoot point
        if (shootPoint != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(shootPoint.position, 0.1f);

            // Draw shooting direction
            if (Application.isPlaying && player != null)
            {
                Gizmos.color = Color.red;
                Vector3 direction = (player.position - shootPoint.position).normalized;
                Gizmos.DrawRay(shootPoint.position, direction * 3f);
            }
        }
    }
}