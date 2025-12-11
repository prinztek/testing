using UnityEngine;

public class AncientBoss : MonoBehaviour
{
    public enum BossState { Wake, Idle, Patrol, Turning, Death }
    private BossState _state = BossState.Wake;

    [Header("Boss Settings")]
    public float moveSpeed = 2.5f;
    public float idleTime = 1.2f;
    public float patrolDistance = 5f;

    [Header("References")]
    [SerializeField] private Transform visualRoot;   // <-- Child containing Animator + SpriteRenderer
    [SerializeField] private Animator _anim;
    [SerializeField] private SpriteRenderer _renderer;

    // RIGHT-FACING animation names (default)
    private static readonly int WakeHash = Animator.StringToHash("wake");
    private static readonly int IdleHash = Animator.StringToHash("idle");
    private static readonly int MoveHash = Animator.StringToHash("run");
    private static readonly int DeathHash = Animator.StringToHash("dead");
    private static readonly int TurnLeftHash = Animator.StringToHash("turnLeft");
    private static readonly int TurnRightHash = Animator.StringToHash("turnRight");

    // LEFT-FACING versions
    private static readonly int Idle_L_Hash = Animator.StringToHash("idle_L");
    private static readonly int Move_L_Hash = Animator.StringToHash("run_L");
    private static readonly int Wake_L_Hash = Animator.StringToHash("wake_L");
    private static readonly int Death_L_Hash = Animator.StringToHash("dead_L");
    private static readonly int TurnLeft_L_Hash = Animator.StringToHash("turnLeft_L");
    private static readonly int TurnRight_L_Hash = Animator.StringToHash("turnRight_L");

    // Patrol
    private Vector3 leftPoint;
    private Vector3 rightPoint;
    private bool movingRight = true;

    // Direction tracking
    private bool facingRight = true; // default facing right

    private float idleTimer = 0f;

    private void Awake()
    {
        if (_anim == null) _anim = visualRoot.GetComponent<Animator>();
        if (_renderer == null) _renderer = visualRoot.GetComponent<SpriteRenderer>();

        leftPoint = transform.position + Vector3.left * patrolDistance;
        rightPoint = transform.position + Vector3.right * patrolDistance;
    }

    private void Start()
    {
        PlayAnimation(WakeHash);
    }

    private void Update()
    {
        switch (_state)
        {
            case BossState.Wake: HandleWake(); break;
            case BossState.Idle: HandleIdle(); break;
            case BossState.Patrol: HandlePatrol(); break;
            case BossState.Turning: HandleTurn(); break;
        }
    }

    // ------------------------
    // WAKE
    // ------------------------
    private void HandleWake()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            ChangeState(BossState.Idle);
            PlayIdle();
        }
    }

    // ------------------------
    // IDLE
    // ------------------------
    private void HandleIdle()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleTime)
        {
            idleTimer = 0f;
            ChangeState(BossState.Patrol);
            PlayRun();
        }
    }

    // ------------------------
    // PATROL MOVEMENT
    // ------------------------
    private void HandlePatrol()
    {
        float step = moveSpeed * Time.deltaTime;

        if (movingRight)
        {
            transform.position = Vector3.MoveTowards(transform.position, rightPoint, step);

            if (transform.position.x >= rightPoint.x)
            {
                movingRight = false;
                StartTurnAnimation(turningRightToLeft: true);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, leftPoint, step);

            if (transform.position.x <= leftPoint.x)
            {
                movingRight = true;
                StartTurnAnimation(turningRightToLeft: false);
            }
        }
    }

    // ------------------------
    // TURN ANIMATION
    // ------------------------
    private void StartTurnAnimation(bool turningRightToLeft)
    {
        ChangeState(BossState.Turning);

        if (turningRightToLeft)
        {
            // was facing right → now face left
            facingRight = false;
            PlayAnimation(TurnLeft_L_Hash);
        }
        else
        {
            // was facing left → now face right
            facingRight = true;
            PlayAnimation(TurnRightHash);
        }
    }

    private void HandleTurn()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            ChangeState(BossState.Patrol);
            PlayRun();
        }
    }

    // ------------------------
    // ANIMATION HELPERS
    // ------------------------
    private void PlayIdle()
    {
        if (facingRight) PlayAnimation(IdleHash);
        else PlayAnimation(Idle_L_Hash);
    }

    private void PlayRun()
    {
        if (facingRight) PlayAnimation(MoveHash);
        else PlayAnimation(Move_L_Hash);
    }

    private void PlayAnimation(int hash)
    {
        _anim.CrossFade(hash, 0.1f);
    }

    // ------------------------
    // DEATH
    // ------------------------
    public void Die()
    {
        ChangeState(BossState.Death);

        if (facingRight) PlayAnimation(DeathHash);
        else PlayAnimation(Death_L_Hash);

        this.enabled = false;
    }

    private void ChangeState(BossState newState)
    {
        _state = newState;
    }
}
