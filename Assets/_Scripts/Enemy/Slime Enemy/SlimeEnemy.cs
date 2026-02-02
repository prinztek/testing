using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    // ========================================
    // STATES
    // ========================================
    public enum State
    {
        Idle,
        Patrol,
        Approach,
        Attack,
        Recovery,
        Death,
        Stunned
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
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private int damage = 10;
    private int currentHealth;
    private bool isDead;

    // ========================================
    // PATROL
    // ========================================
    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 1.5f;

    // ========================================
    // COMBAT
    // ========================================
    [Header("Combat")]
    [SerializeField] private float idleDuration = 1.5f;      // Time to pause at walls/ledges
    [SerializeField] private float approachSpeed = 2.5f;
    [SerializeField] private float detectRange = 3f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackDuration = 0.6f;
    [SerializeField] private float recoveryTime = 0.8f;
    [SerializeField] private float stunDuration = 2.0f;      // How long stun lasts


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
    private Transform player;
    private bool shouldFlipAfterIdle = false;  // Track if we should flip when idle ends

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

        // Check for stun before anything else
        if (enemyStats.activeStatus != null && enemyStats.IsStunned() == true)
        {
            if (currentState != State.Stunned)
                ChangeState(State.Stunned);
        }

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                Idle();
                // Check for player during idle first
                if (PlayerDetected())
                {
                    ChangeState(State.Approach);
                }
                else
                {
                    Idle();  // Only process idle logic if no player
                }
                break;
            case State.Patrol:
                Patrol();

                if (PlayerDetected())
                {
                    ChangeState(State.Approach);
                }

                break;

            case State.Approach:
                MoveTowardPlayer();

                if (InAttackRange())
                {
                    ChangeState(State.Attack);
                }
                else if (!PlayerDetected())
                {
                    ChangeState(State.Patrol);
                }
                break;

            case State.Attack:
                if (stateTimer >= attackDuration)
                    ChangeState(State.Recovery);
                break;

            case State.Recovery:
                if (stateTimer >= recoveryTime)
                    ChangeState(State.Patrol);
                break;

            case State.Stunned:
                // Just wait out the stun
                if (enemyStats.IsStunned() == false)
                {
                    // After stun, decide what to do based on player position
                    if (InAttackRange())
                    {
                        ChangeState(State.Attack);
                    }
                    else if (!PlayerDetected())
                    {
                        ChangeState(State.Patrol);
                    }
                    break;
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

            case State.Approach:
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

        if (enemyStats.IsStunned() == true)

            if (enemyStats.Grounded() == false)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                return;
            }

        int dir = facingRight ? 1 : -1;

        // Edge detection using EnemyStats
        if (enemyStats.HasGroundAhead(dir) == false)
        {
            // Mark that we should flip after idling
            shouldFlipAfterIdle = true;
            ChangeState(State.Idle);
            return;
        }

        rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);
    }

    private void MoveTowardPlayer()
    {
        if (player == null) return;

        if (enemyStats.Grounded() == false)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        int dir = player.position.x < transform.position.x ? -1 : 1;
        FaceDirection(dir);

        rb.linearVelocity = new Vector2(dir * approachSpeed, rb.linearVelocity.y);
    }

    private bool PlayerDetected()
    {
        if (player == null) return false;

        bool isPlayerDetected = false;
        float distance = Mathf.Abs(player.position.x - rb.position.x);

        // if the distance between the player and slime is less than detection range
        // player is detected
        if (distance <= detectRange)
        {
            isPlayerDetected = true;
        }
        else
        {
            isPlayerDetected = false;
        }

        int dirToPlayer = player.position.x < transform.position.x ? -1 : 1;
        Debug.Log(dirToPlayer == 1 ? "face right" : "face left");
        bool isGroundAhead = enemyStats.HasGroundAhead(dirToPlayer);
        Debug.Log("isGroundAhaed: " + isGroundAhead);

        return isPlayerDetected;
    }

    private bool InAttackRange()
    {
        if (player == null) return false;

        float distance = Mathf.Abs(player.position.x - transform.position.x);
        return distance <= attackRange;
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
        // ===== Ground Ahead (MAGENTA) =====
        if (enemyStats != null && enemyStats.groundCheckPoint != null)
        {
            int dir = facingRight ? 1 : -1;

            Vector3 origin =
                enemyStats.groundCheckPoint.position +
                Vector3.right * dir * enemyStats.groundCheckX;

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(origin, origin + Vector3.down * enemyStats.groundCheckY);
        }

        // ===== Detect Range (YELLOW) =====
        Gizmos.color = Color.yellow;

        Vector3 pos = transform.position;

        Gizmos.DrawLine(
            pos + Vector3.left * detectRange,
            pos + Vector3.right * detectRange
        );

        // // ===== Attack Range (RED) =====
        // Gizmos.color = Color.red;

        // Gizmos.DrawLine(
        //     pos + Vector3.left * attackRange,
        //     pos + Vector3.right * attackRange
        // );
    }
}
