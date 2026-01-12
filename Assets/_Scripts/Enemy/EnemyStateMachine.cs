using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    [HideInInspector] public EnemyStatsNew stats;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform player;

    // Public states
    public EnemyIdleState idleState = new EnemyIdleState();
    public EnemyPatrolState patrolState = new EnemyPatrolState();
    public EnemyChaseState chaseState = new EnemyChaseState();
    public EnemyAttackState attackState = new EnemyAttackState();
    public EnemyHurtState hurtState = new EnemyHurtState();
    public EnemyDeathState deathState = new EnemyDeathState();
    public EnemyReturnState returnState = new EnemyReturnState();

    private EnemyBaseState currentState;
    public EnemyBaseState LastState { get; private set; }
    [HideInInspector] public bool movingRight = true;
    [HideInInspector] public bool isFacingRight = true;
    [HideInInspector] public Vector2 lastHitDirection;
    // Inside EnemyStateMachine.cs
    [SerializeField] public float knockbackForce = 3f;
    [SerializeField] public float verticalForce = 2f;
    [SerializeField] private GameObject attackCollider;
    // Gizmo settings
    [Header("Gizmo Visualizations")]
    [SerializeField] private bool showChaseRange = true;
    [SerializeField] private bool showAttackRange = true;

    [HideInInspector] public EnemyEnvironmentSensor sensor;
    [Header("Territory")]
    [SerializeField] public Transform leftLimit;
    [SerializeField] public Transform rightLimit;

    [HideInInspector] public Vector2 homePosition;

    void Awake()
    {
        stats = GetComponent<EnemyStatsNew>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        sensor = GetComponent<EnemyEnvironmentSensor>();  // Add this line

        homePosition = transform.position;
    }

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


    void Start()
    {
        // Start with idle
        TransitionToState(idleState);
    }

    void Update()
    {
        currentState?.UpdateState(this);
    }

    public void SwitchState(EnemyBaseState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
            LastState = currentState;
        }

        currentState = newState;
        currentState.EnterState(this);
    }

    public void TransitionToState(EnemyBaseState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
            LastState = currentState;
        }

        currentState = newState;
        currentState.EnterState(this);
    }

    public void FlipDirection(bool shouldFaceRight)
    {
        if (shouldFaceRight != isFacingRight)
        {
            isFacingRight = shouldFaceRight;
            movingRight = shouldFaceRight; // ✅ UPDATE movement logic

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (shouldFaceRight ? 1 : -1);
            transform.localScale = scale;
        }
    }


    private void OnDrawGizmos()
    {
        if (stats == null) return;

        if (showChaseRange)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stats.DetectionRange);
        }

        if (showAttackRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stats.AttackRange);
        }
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;

        float distance = Vector2.Distance(transform.position, player.position);

        return distance <= stats.detectionRange;   // No facing direction required
    }
    public bool IsOutsideTerritory()
    {
        float x = transform.position.x;
        return x < leftLimit.position.x || x > rightLimit.position.x;
    }

    public bool PlayerLeftTerritory()
    {
        if (player == null) return true;

        float px = player.position.x;
        return px < leftLimit.position.x || px > rightLimit.position.x;
    }


    // private void OnDrawGizmosSelected()
    // {
    //     if (stats == null) return;

    //     Gizmos.color = Color.violet;

    //     // Draw the actual horizontal attack range
    //     Gizmos.DrawWireSphere(transform.position, stats.AttackRange);

    //     // ALSO draw the "vertical window" for attack
    //     Vector3 top = transform.position + Vector3.up * 3f;
    //     Vector3 bottom = transform.position + Vector3.down * 3f;
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawLine(top + Vector3.left * stats.AttackRange, top + Vector3.right * stats.AttackRange);
    //     Gizmos.DrawLine(bottom + Vector3.left * stats.AttackRange, bottom + Vector3.right * stats.AttackRange);
    //     Gizmos.DrawLine(top + Vector3.left * stats.AttackRange, bottom + Vector3.left * stats.AttackRange);
    //     Gizmos.DrawLine(top + Vector3.right * stats.AttackRange, bottom + Vector3.right * stats.AttackRange);
    // }

}
