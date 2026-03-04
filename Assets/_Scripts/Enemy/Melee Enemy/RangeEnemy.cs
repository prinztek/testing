using UnityEngine;

public class RangeEnemy : MonoBehaviour
{
    // ========================================
    // STATES
    // ========================================
    public enum State
    {
        Idle,           // Standing still, will return to patrol
        Patrol,
        Retreat,        // Player too close, back away
        Attack,         // Aim and shoot
        Recovery,       // After shooting
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
    [SerializeField] private Transform shootPoint;
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
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float retreatRange = 2.5f;      // If player closer, try to retreat
    [SerializeField] private float maxShootRange = 7f;

    // ========================================
    // MOVEMENT
    // ========================================
    [Header("Movement")]
    [SerializeField] private float retreatSpeed = 3f;

    // ========================================
    // COMBAT TIMING
    // ========================================
    [Header("Combat Timing")]
    [SerializeField] private float idleDuration = 1.5f;      // Time to pause at walls/ledges
    [SerializeField] private float attackDuration = 1.0f;
    [SerializeField] private float recoveryTime = 1.0f;
    [SerializeField] private float stunDuration = 2.0f;      // How long stun lasts

    // ========================================
    // BEHAVIOR SETTINGS
    // ========================================
    [Header("Behavior")]
    [SerializeField] private bool canRetreatOffLedges = false;

    // ========================================
    // ANIMATIONS
    // ========================================
    private static readonly int IdleHash = Animator.StringToHash("idle");
    private static readonly int MoveHash = Animator.StringToHash("run");
    private static readonly int AttackHash = Animator.StringToHash("rangeAttack");

    // ========================================
    // STATE DATA
    // ========================================
    private State currentState;
    private float stateTimer;
    private bool facingRight = true;
    private Transform player;
    private bool isCornered = false;  // Track if rat is backed against wall
    private bool shouldFlipAfterIdle = false;  // Track if we should flip when idle ends

    // ========================================
    // UNITY
    // ========================================
    private void HandleOnDeath(EnemyStats enemyStats)
    {
        // ignore the parameter
        Debug.Log(" Enemy died, dropping loot and playing death effects.");
        // Play death effects
        // ChangeState(State.Death);
        // Destroy(gameObject);  // Delay to allow death animation/effects to play
    }

    private void Awake()
    {
        enemyStats ??= GetComponent<EnemyStats>();
        animator ??= GetComponentInChildren<Animator>();
        spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
        visual ??= spriteRenderer.transform;
        rb = GetComponent<Rigidbody2D>();

        currentHealth = enemyStats.GetMaxHealth();
        ChangeState(State.Patrol);
    }

    private void Update()
    {
        if (isDead) return;

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                Idle();
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

            case State.Retreat:
                RetreatFromPlayer();

                // If successfully retreated enough, attack
                if (!PlayerTooClose())
                {
                    ChangeState(State.Attack);
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
                    // After stun, decide what to do based on player position
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
                isCornered = false;  // Reset cornered flag when leaving combat
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

            case State.Stunned:
                rb.linearVelocity = Vector2.zero;
                PlayAnimation(IdleHash);  // Or you could add a "stunned" animation
                break;

            case State.Death:
                rb.linearVelocity = Vector2.zero;
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
        // You can add wall check here if you have it: bool isWall = enemyStats.IsFacingWall(dir);

        if (noGroundAhead) // || isWall
        {
            // Mark that we should flip after idling
            shouldFlipAfterIdle = true;
            ChangeState(State.Idle);
            return;
        }

        rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);
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
            // CORNERED! Mark as cornered and force attack
            rb.linearVelocity = Vector2.zero;
            isCornered = true;
            ChangeState(State.Attack);
            return;
        }

        // Can retreat - back away from player
        FacePlayer();
        rb.linearVelocity = new Vector2(retreatDir * retreatSpeed, rb.linearVelocity.y);
    }

    private void DecideNextAction()
    {
        if (player == null || !PlayerInRange())
        {
            ChangeState(State.Patrol);
            return;
        }

        // KEY FIX: If cornered, don't try to retreat again - just attack!
        if (isCornered)
        {
            ChangeState(State.Attack);
            return;
        }

        // Normal behavior: retreat if too close, otherwise attack
        if (PlayerTooClose())
        {
            ChangeState(State.Retreat);
        }
        else
        {
            ChangeState(State.Attack);
        }
    }

    // ========================================
    // RANGE CHECKS
    // ========================================
    private bool PlayerInRange()
    {
        if (player == null) return false;
        float distance = Mathf.Abs(player.position.x - transform.position.x);
        return distance <= detectionRange;
    }

    private bool PlayerTooClose()
    {
        if (player == null) return false;
        float distance = Mathf.Abs(player.position.x - transform.position.x);
        return distance < retreatRange;
    }

    // ========================================
    // SHOOTING (Called by Animation Event)
    // ========================================
    public void OnShootArrow()
    {
        if (currentState != State.Attack) return;
        if (player == null) return;

        // Once we shoot, we're no longer cornered (player might back off)
        isCornered = false;

        Vector2 dir = player.position.x < transform.position.x ? Vector2.left : Vector2.right;
        EnemyProjectile proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        proj.SetDamage(damage, gameObject);
        proj.Launch(dir * projectileSpeed);

        float angle = dir.x > 0 ? 0 : 180;
        proj.transform.rotation = Quaternion.Euler(0, 0, angle);
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
        enemyStats.OnDeath += HandleOnDeath;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
        enemyStats.OnDeath -= HandleOnDeath;
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

            if (Application.isPlaying && player != null)
            {
                Gizmos.color = Color.red;
                Vector3 direction = (player.position - shootPoint.position).normalized;
                Gizmos.DrawRay(shootPoint.position, direction * 3f);
            }
        }
    }
}