using UnityEngine;
using System.Collections;

public class Boss2 : MonoBehaviour
{
    public enum BossState { Idle, Chase, AttackWindup, AttackSwing, Death }
    private BossState _state = BossState.Idle;

    [Header("Boss Settings")]
    public float moveSpeed = 3f;
    public float attackRange = 2.5f;
    public float detectionRange = 8f;
    public float windupTime = 0.333f;
    public float swingTime = 0.667f;
    public float attackCooldown = 3f;
    public float attackDamage = 20f;

    [Header("References")]
    [SerializeField] private Animator _anim;
    [SerializeField] private SpriteRenderer _renderer;
    public Transform player;

    private bool _isAttacking;
    private bool _facingRight = true;
    private float _lastAttackTime;

    // --- Animator States (match your animation clip names)
    private static readonly int Idle = Animator.StringToHash("idle");
    private static readonly int Walk = Animator.StringToHash("running");
    private static readonly int Windup = Animator.StringToHash("charge");
    private static readonly int Attack = Animator.StringToHash("attack1");
    private static readonly int Death = Animator.StringToHash("dead");
    private int _currentAnimState;

    private void Awake()
    {
        if (_anim == null) _anim = GetComponentInChildren<Animator>();
        if (_renderer == null) _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (player == null || _state == BossState.Death) return;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (_state)
        {
            case BossState.Idle:
                PlayAnimation(Idle);
                if (distance < detectionRange)
                {
                    ChangeState(BossState.Chase);
                }
                break;

            case BossState.Chase:
                ChasePlayer(distance);
                break;

            case BossState.AttackWindup:
            case BossState.AttackSwing:
                // waiting for timers to finish in AttackRoutine
                break;
        }

        // Flip to face player
        if (player != null)
        {
            bool shouldFaceRight = player.position.x > transform.position.x;
            if (shouldFaceRight != _facingRight)
            {
                _facingRight = shouldFaceRight;
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (_facingRight ? 1 : -1);
                transform.localScale = scale;
            }
        }


    }




    private void ChasePlayer(float distance)
    {
        if (_isAttacking) return;

        Vector2 dir = (player.position - transform.position).normalized;

        // Stop at attack range before attacking
        float stopDistance = attackRange * 0.8f;

        if (distance <= stopDistance && Time.time > _lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackRoutine());
            return;
        }

        if (distance > stopDistance)
        {
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
            PlayAnimation(Walk);
        }
        else
        {
            // Stop moving and idle until attack triggers
            PlayAnimation(Idle);
        }
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        _lastAttackTime = Time.time;

        // --- WINDUP ---
        ChangeState(BossState.AttackWindup);
        PlayAnimation(Windup);
        yield return new WaitForSeconds(windupTime);

        // --- SWING ---
        ChangeState(BossState.AttackSwing);
        PlayAnimation(Attack);
        yield return new WaitForSeconds(swingTime);

        // --- RETURN TO CHASE ---
        ChangeState(BossState.Chase);
        _isAttacking = false;
    }

    public void Die()
    {
        ChangeState(BossState.Death);
        PlayAnimation(Death);
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    // -----------------------------
    // HELPERS
    // -----------------------------
    private void ChangeState(BossState newState)
    {
        _state = newState;
    }

    private void PlayAnimation(int state)
    {
        if (_currentAnimState == state) return;
        _anim.CrossFade(state, 0, 0);
        _currentAnimState = state;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.2f, 0.2f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = new Color(0f, 0.7f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
#if UNITY_EDITOR
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 18;
        style.fontStyle = FontStyle.Bold;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, $"State: {_state}", style);
#endif
    }
}
