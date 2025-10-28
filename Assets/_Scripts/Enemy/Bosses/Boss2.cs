using UnityEngine;
using System.Collections;
using System;

public class Boss2 : MonoBehaviour
{
    [Header("Boss Stats")]
    public int maxHealth = 200;
    public int currentHealth;
    [Header("References")]
    public BossAttackHitbox bossAttackHitbox;

    public enum BossState { Idle, Chase, AttackWindup, AttackSwing, Taunt, Death }
    private BossState _state = BossState.Idle;

    [Header("Boss Settings")]
    public float moveSpeed = 3f;
    public float attackRange = 2.5f;
    public float detectionRange = 8f;
    public float windupTime = 0.333f;
    public float swingTime = 0.3335f; // 0.667f;
    public float swingTime2 = 0.667f;
    public float attackCooldown = 3f;
    public int attackDamage = 20;

    [Header("Personality")]
    [Range(0f, 1f)] public float aggression = 0.6f; // how often it attacks early
    [Range(0f, 1f)] public float patience = 0.4f;  // how long it waits before taunting
    [Range(0f, 1f)] public float curiosity = 0.3f; // how often it "investigates" when bored

    private float _idleTimer;
    private float _nextBoredTime;
    private bool _isAttacking;
    private bool _facingRight = true;
    private float _lastAttackTime;
    private int _currentAnimState;

    [Header("References")]
    [SerializeField] private Animator _anim;
    [SerializeField] private SpriteRenderer _renderer;
    public Transform player;

    // Animator hashes
    private static readonly int Idle = Animator.StringToHash("idle");
    private static readonly int Walk = Animator.StringToHash("running");
    private static readonly int Windup = Animator.StringToHash("charge");
    private static readonly int Attack = Animator.StringToHash("attack1");
    private static readonly int Attack2 = Animator.StringToHash("attack2");
    private static readonly int Attack3 = Animator.StringToHash("attack3");
    private static readonly int Death = Animator.StringToHash("dead");

    void Start()
    {
        currentHealth = maxHealth;  // Set initial health to max health
    }
    private void Awake()
    {
        if (_anim == null) _anim = GetComponentInChildren<Animator>();
        if (_renderer == null) _renderer = GetComponentInChildren<SpriteRenderer>();

        // Give each boss instance slightly unique personality
        aggression += UnityEngine.Random.Range(-0.15f, 0.15f);
        patience += UnityEngine.Random.Range(-0.15f, 0.15f);
        curiosity += UnityEngine.Random.Range(-0.15f, 0.15f);

        aggression = Mathf.Clamp01(aggression);
        patience = Mathf.Clamp01(patience);
        curiosity = Mathf.Clamp01(curiosity);

        _nextBoredTime = UnityEngine.Random.Range(3f, 6f) * (1f + patience);
    }

    private void Update()
    {
        if (player == null || _state == BossState.Death) return;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (_state)
        {
            case BossState.Idle:
                HandleIdle(distance);
                break;
            case BossState.Chase:
                HandleChase(distance);
                break;
            case BossState.AttackWindup:
            case BossState.AttackSwing:
                break;
            case BossState.Taunt:
                // Taunt coroutine handles behavior
                break;
        }

        FacePlayer();
    }

    private void HandleIdle(float distance)
    {
        PlayAnimation(Idle);
        _idleTimer += Time.deltaTime;

        // Spot player
        if (distance < detectionRange)
        {
            ChangeState(BossState.Chase);
            _idleTimer = 0;
            return;
        }

        // UnityEngine.Random chance to start pacing if bored
        if (_idleTimer > _nextBoredTime && UnityEngine.Random.value < curiosity)
        {
            StartCoroutine(TauntRoutine("bored"));
            _idleTimer = 0;
            _nextBoredTime = UnityEngine.Random.Range(4f, 8f) * (1f + patience);
        }
    }

    private void HandleChase(float distance)
    {
        if (_isAttacking) return;

        Vector2 dir = (player.position - transform.position).normalized;
        float stopDistance = attackRange * (0.7f + aggression * 0.3f); // aggressive bosses stop closer

        // Attack decision
        if (distance <= stopDistance && Time.time > _lastAttackTime + attackCooldown)
        {
            // Small chance the boss fakes an attack (taunt instead)
            if (UnityEngine.Random.value < (1f - aggression) * 0.2f)
            {
                StartCoroutine(TauntRoutine("fake"));
            }
            else
            {
                StartCoroutine(AttackRoutine());
            }
            return;
        }

        // Move or idle
        if (distance > stopDistance)
        {
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
            PlayAnimation(Walk);
        }
        else
        {
            PlayAnimation(Idle);
        }
    }

    private IEnumerator AttackRoutine()
    {
        bossAttackHitbox.ResetHits();
        _isAttacking = true;
        _lastAttackTime = Time.time;

        ChangeState(BossState.AttackWindup);
        PlayAnimation(Windup);
        yield return new WaitForSeconds(windupTime);


        ChangeState(BossState.AttackSwing);
        int rand = UnityEngine.Random.Range(0, 3);

        if (rand == 0)
        {
            PlayAnimation(Attack);
            yield return new WaitForSeconds(swingTime);
        }
        else if (rand == 1)
        {
            PlayAnimation(Attack2);
            yield return new WaitForSeconds(swingTime);
        }
        else
        {
            PlayAnimation(Attack3);
            yield return new WaitForSeconds(swingTime2);
        }


        int rand2 = UnityEngine.Random.Range(0, 2);
        if (rand2 == 0)
        {
            ChangeState(BossState.Chase);
        }
        else
        {
            ChangeState(BossState.Idle);
        }
        _isAttacking = false;
    }

    private IEnumerator TauntRoutine(string type)
    {
        ChangeState(BossState.Taunt);
        PlayAnimation(Walk);

        // Configurable pacing behavior
        float moveDuration = 1.5f;   // how long it walks each direction
        int cycles = 2;              // how many times it goes back and forth
        float dir = UnityEngine.Random.value < 0.5f ? -1f : 1f; // start randomly left or right

        // Save starting position to ensure it doesn’t drift
        Vector3 origin = transform.position;

        for (int i = 0; i < cycles; i++)
        {
            float timer = 0f;

            while (timer < moveDuration)
            {
                transform.position += Vector3.right * dir * moveSpeed * 0.5f * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }

            // Flip direction visually and logically
            dir *= -1f;
            Flip();
        }

        // Snap back close to original position (prevents drifting)
        transform.position = new Vector3(origin.x, transform.position.y, transform.position.z);

        // Return to idle after taunting
        ChangeState(BossState.Idle);
        PlayAnimation(Idle);
        _idleTimer = 0;
    }


    public void Die()
    {
        ChangeState(BossState.Death);
        PlayAnimation(Death);
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    private void FacePlayer()
    {
        if (player == null || _state == BossState.Taunt) return;

        bool shouldFaceRight = player.position.x > transform.position.x;
        if (shouldFaceRight != _facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        _facingRight = !_facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void ChangeState(BossState newState) => _state = newState;

    private void PlayAnimation(int state)
    {
        if (_currentAnimState == state) return;
        _anim.CrossFade(state, 0, 0);
        _currentAnimState = state;
    }

    public void TakeDamage(int damageAmount, Vector2 attackerPosition, bool doScreenShake = true)
    {
        currentHealth -= damageAmount;  // Decrease health by the damage amount
        Debug.Log($"Boss took {damageAmount} damage. Health now: {currentHealth}");

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
