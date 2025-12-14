using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class AncientBoss : MonoBehaviour
{
    // ========================================
    // ENUMS
    // ========================================
    public enum State { Wake, Idle, Patrol, Turn, Chase, Attack, Hit, Death }

    // ========================================
    // SETTINGS
    // ========================================
    [Header("Movement")]
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
    public float detectionRange = 6f; // Distance to detect player
    public LayerMask playerLayer;
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float rangedRange = 8f;
    [SerializeField] private float meleeAttackDuration = 1.2f;
    [SerializeField] private float rangedAttackDuration = 1.75f;

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform visual;
    public GameObject AncientBossHUD;

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
    private static readonly int WakeHash = Animator.StringToHash("wake");
    private static readonly int IdleHash = Animator.StringToHash("idle");
    private static readonly int RunHash = Animator.StringToHash("run");
    private static readonly int MeleeAttackHash = Animator.StringToHash("meleeAttack");
    private static readonly int RangeAttackHash = Animator.StringToHash("rangeAttack");
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
    private float attackTimer;
    private bool isDead = false;
    private bool isTurning = false;

    public int damage = 20;
    public int GetDamage() { return damage; }

    private enum AttackType { None, Melee, Ranged }
    private AttackType currentAttack;
    // ========================================
    // INITIALIZATION
    // ========================================
    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (visual == null) visual = spriteRenderer.transform;

        // Set patrol bounds
        Vector3 startPos = transform.position;
        leftBound = startPos + Vector3.left * patrolDistance;
        rightBound = startPos + Vector3.right * patrolDistance;

        currentHealth = maxHealth;
        ChangeState(State.Wake);
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
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        player = playerObj.transform;
    }

    // ========================================
    // UPDATE
    // ========================================
    private void Update()
    {
        if (isDead) return;

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
        transform.position = Vector3.MoveTowards(transform.position, target, patrolSpeed * Time.deltaTime);

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
                    ChangeState(State.Chase);
                }
                break;

            case AttackType.Ranged:
                if (stateTimer >= rangedAttackDuration)
                {
                    attackTimer = 0f;
                    ChangeState(State.Chase);
                }
                break;
        }
    }

    private void ChooseAttack()
    {
        stateTimer = 0f;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= meleeRange)
        {
            currentAttack = AttackType.Melee;
            PlayAnimation(MeleeAttackHash);
        }
        else
        {
            currentAttack = AttackType.Ranged;
            PlayAnimation(RangeAttackHash);
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

        Vector3 characterOrigin = transform.position + Vector3.right * (facingRight ? 2 : -2);
        float distanceToRealPlayerOrigin = Vector3.Distance(characterOrigin, player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

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
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;

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
                break;
        }
    }

    private void OnStateExit(State state) { }
    #endregion

    #region COMBAT
    private bool PlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= range;
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

    #region HEALTH
    public void TakeDamage(int damageAmount, Vector2 attackerPosition, bool doScreenShake = true)
    {
        Debug.Log($"Ancient Boss took {damageAmount} damage.");

        if (AncientBossHUD != null && !AncientBossHUD.activeSelf)
            AncientBossHUD.SetActive(true);

        currentHealth -= damageAmount;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
            ChangeState(State.Death);
    }
    #endregion

    #region GIZMOS
    private void OnDrawGizmosSelected()
    {
        // Vector3 center = transform.position + Vector3.right * (facingRight ? -2 : 2);
        Vector3 center = transform.position;

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
    }
    #endregion
}
