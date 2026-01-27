using System.Collections;
using UnityEngine;

public class TheAncientBoss : MonoBehaviour
{
    [Header("Component References:")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform visual;
    public Rigidbody2D rb;
    public GameObject AncientBossHUD;
    public Transform centerTransform;

    [Header("Sprite Offset")]
    public float flipOffsetX = -4.4f;

    [Header("Ground Check Settings:")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Movement & Combat")]
    public float speed = 3f;
    public float movementSpeed;
    public float attackRange = 2f;
    public float attackTimer = 3f;

    private bool facingRight = true;
    private bool isAlive = true;
    private Transform player;
    private CharacterStats playerStats;

    // Animation hashes
    private static readonly int IdleHash = Animator.StringToHash("idle");
    private static readonly int RunHash = Animator.StringToHash("run");
    private static readonly int MeleeAttackHash = Animator.StringToHash("meleeAttack");

    public enum State { Idle, Run, Attack }
    private State currentState;

    [HideInInspector] public bool isAttacking;
    [HideInInspector] public float attackCountdown;

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
        isAlive = true;
    }

    private void Start()
    {
        ChangeState(State.Idle);
    }

    private void Update()
    {
        if (!isAlive) return;

        // Only run behavior if player exists
        if (player == null) return;

        switch (currentState)
        {
            case State.Idle: IdleState(); break;
            case State.Run: RunState(); break;
        }

        if (!isAttacking)
        {
            attackCountdown -= Time.deltaTime;
        }
    }

    private void ChangeState(State newState)
    {
        ExitState(currentState);
        currentState = newState;
        OnStateEnter(newState);
    }

    private void OnStateEnter(State state)
    {
        switch (state)
        {
            case State.Idle:
                PlayAnimation(IdleHash);
                break;
            case State.Run:
                PlayAnimation(RunHash);
                break;
        }
    }

    private void ExitState(State state)
    {
        // optional cleanup
    }

    private void IdleState()
    {
        rb.linearVelocity = Vector2.zero;
        RunToPlayer();

        if (attackCountdown <= 0)
        {
            AttackHandler();
            attackCountdown = attackTimer;
        }
    }

    private void RunState()
    {
        TargetPlayerPosition();

        if (attackCountdown <= 0)
        {
            AttackHandler();
            attackCountdown = attackTimer;
        }
    }

    private void RunToPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(player.position, rb.position);
        if (distance >= attackRange)
        {
            PlayAnimation(RunHash);
            ChangeState(State.Run);
        }
    }

    private void TargetPlayerPosition()
    {
        if (player == null) return;

        if (Grounded())
        {
            Flip();
            Vector2 targetPos = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            if (Vector2.Distance(player.position, rb.position) <= attackRange)
            {
                ChangeState(State.Idle);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -25f);
        }
    }

    public void Flip()
    {
        if (player == null) return;

        bool shouldFaceLeft = player.position.x < centerTransform.position.x;

        if (shouldFaceLeft && facingRight)
            SetFacingRight(false);
        else if (!shouldFaceLeft && !facingRight)
            SetFacingRight(true);
    }

    private void SetFacingRight(bool faceRight)
    {
        facingRight = faceRight;

        Vector3 eulers = visual.localEulerAngles;
        eulers.y = faceRight ? 0f : 180f;
        visual.localEulerAngles = eulers;

        Vector3 pos = visual.localPosition;
        pos.x = faceRight ? 0f : flipOffsetX;
        visual.localPosition = pos;
    }

    public bool Grounded()
    {
        if (groundCheckPoint == null) return false;

        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);
    }

    #region Attacking
    public void AttackHandler()
    {
        if (player == null) return;

        float distance = Vector2.Distance(player.position, rb.position);
        if (distance <= attackRange && !isAttacking)
        {
            if (currentAttackCoroutine != null)
                StopCoroutine(currentAttackCoroutine);

            currentAttackCoroutine = StartCoroutine(GroundStomp());
        }
    }

    private IEnumerator GroundStomp()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        PlayAnimation(MeleeAttackHash);
        yield return new WaitForSeconds(1.2f);

        PlayAnimation(MeleeAttackHash);
        yield return new WaitForSeconds(1.2f);

        isAttacking = false;
        currentAttackCoroutine = null;
    }
    #endregion

    private void PlayAnimation(int animHash)
    {
        animator?.CrossFade(animHash, 0f, 0);
    }

    private void StopAnimation(int animHash)
    {
        animator?.StopPlayback();
    }
}
