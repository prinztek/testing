using System;
using Unity.Cinemachine;
using UnityEngine;

public class ShadowOfStormsBoss : MonoBehaviour
{
    // ========================================
    // STATES
    // ========================================
    public enum State
    {
        Idle,
        Approach,
        Attack,
        Recovery,
        Death
    }

    public enum BossAttackType
    {
        ChargeExplosion,
        ChargeBeam,
        ComboAttack
    }

    // ========================================
    // REFERENCES
    // ========================================
    [Header("References")]
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform centerTransform;

    // ========================================
    // VFX
    // ========================================
    [Header("VFX")]
    [SerializeField] private GameObject attacksVFX;
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject chargeBeamVFX;
    [SerializeField] private GameObject chargeExplosionVFX;
    [SerializeField] private GameObject beam;

    // ========================================
    // TIMING
    // ========================================
    [Header("Timing")]
    [SerializeField] private float idleTime = 1f;
    [SerializeField] private float attackDuration = 1.2f;
    [SerializeField] private float recoveryTime = 0.8f;

    // ========================================
    // MOVEMENT
    // ========================================
    [Header("Movement")]
    [SerializeField] private float approachSpeed = 4.5f;

    // ========================================
    // ATTACK RANGES
    // ========================================
    [Header("Attack Ranges")]
    [SerializeField] private float closeRange = 1.2f;
    [SerializeField] private float midRange = 2.5f;
    [SerializeField] private float farRange = 5f;

    // ========================================
    // ANIMATION HASHES
    // ========================================
    private static readonly int IdleHash = Animator.StringToHash("idle");
    private static readonly int RunHash = Animator.StringToHash("run");
    private static readonly int DeathHash = Animator.StringToHash("dead");
    private static readonly int ChargeExplosionHash = Animator.StringToHash("chargeExplosion");
    private static readonly int ChargeBeamHash = Animator.StringToHash("chargeBeam");
    private static readonly int ComboHash = Animator.StringToHash("meleeAttack");

    // ========================================
    // INTERNAL
    // ========================================
    private State currentState;
    private BossAttackType currentAttack;

    private Transform player;
    private CharacterStats playerStats;

    private float stateTimer;
    private bool facingRight = true;
    private bool isDead;

    // ========================================
    // UNITY
    // ========================================
    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;

        if (enemyStats != null)
            enemyStats.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;

        if (enemyStats != null)
            enemyStats.OnDeath -= HandleDeath;
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        player = playerObj.transform;
        playerStats = player.GetComponent<CharacterStats>();
    }

    private void Awake()
    {
        enemyStats ??= GetComponent<EnemyStats>();
        animator ??= GetComponentInChildren<Animator>();
        rb ??= GetComponent<Rigidbody2D>();

        ChangeState(State.Idle);
    }

    private void Update()
    {
        if (isDead || player == null) return;

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:

                if (stateTimer >= idleTime)
                {
                    SelectAttack();

                    if (NeedsToCloseGap())
                        ChangeState(State.Approach);
                    else
                        ChangeState(State.Attack);
                }

                break;

            case State.Approach:

                MoveTowardPlayer();

                if (InAttackRange())
                    ChangeState(State.Attack);

                break;

            case State.Attack:

                if (stateTimer >= attackDuration)
                    ChangeState(State.Recovery);

                break;

            case State.Recovery:

                if (stateTimer >= recoveryTime)
                    ChangeState(State.Idle);

                break;
        }
    }

    private void FixedUpdate()
    {
        if (isDead || player == null) return;

        if (currentState == State.Approach)
        {
            MoveTowardPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
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

            case State.Approach:

                PlayAnimation(RunHash);

                break;

            case State.Attack:

                rb.linearVelocity = Vector2.zero;
                LockAttackDirection();
                PlayAttackAnimation();

                break;

            case State.Recovery:

                rb.linearVelocity = Vector2.zero;
                PlayAnimation(IdleHash);

                break;

            case State.Death:

                isDead = true;
                rb.linearVelocity = Vector2.zero;
                PlayAnimation(DeathHash);

                break;
        }
    }

    // ========================================
    // ATTACK SELECTION
    // ========================================
    private void SelectAttack()
    {
        float distance = Mathf.Abs(player.position.x - centerTransform.position.x);

        if (distance <= closeRange)
        {
            int random = UnityEngine.Random.Range(0, 2);
            if (random == 0)
                currentAttack = BossAttackType.ChargeExplosion;
            else
                currentAttack = BossAttackType.ComboAttack;
        }

        else if (distance <= midRange)
            currentAttack = BossAttackType.ChargeExplosion;

        else
            currentAttack = BossAttackType.ChargeBeam;
    }

    private void PlayAttackAnimation()
    {
        switch (currentAttack)
        {
            case BossAttackType.ChargeExplosion:
                PlayAnimation(ChargeExplosionHash);
                break;

            case BossAttackType.ChargeBeam:
                PlayAnimation(ChargeBeamHash);
                break;

            case BossAttackType.ComboAttack:
                PlayAnimation(ComboHash);
                break;
        }
    }

    // ========================================
    // MOVEMENT
    // ========================================
    private void MoveTowardPlayer()
    {
        float dir = player.position.x < centerTransform.position.x ? -1f : 1f;

        rb.linearVelocity = new Vector2(dir * approachSpeed, rb.linearVelocity.y);

        FacePlayer();
    }

    private void MoveTowardsPlayer(float speed)
    {
        float dir = facingRight ? 1f : -1f;

        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }

    // ========================================
    // RANGE CHECKS
    // ========================================
    private bool NeedsToCloseGap()
    {
        float desiredRange = GetDesiredRangeForAttack();
        float distance = Mathf.Abs(player.position.x - centerTransform.position.x);

        return distance > desiredRange;
    }

    private bool InAttackRange()
    {
        float desiredRange = GetDesiredRangeForAttack();
        float distance = Mathf.Abs(player.position.x - centerTransform.position.x);

        return distance <= desiredRange;
    }

    private float GetDesiredRangeForAttack()
    {
        switch (currentAttack)
        {
            case BossAttackType.ComboAttack:
                return closeRange;

            case BossAttackType.ChargeExplosion:
                return midRange;

            case BossAttackType.ChargeBeam:
                return farRange;
        }

        return midRange;
    }

    // ========================================
    // FACING
    // ========================================
    private void LockAttackDirection()
    {
        int dir = player.position.x < centerTransform.position.x ? -1 : 1;
        FaceDirection(dir);
    }

    private void FacePlayer()
    {
        int dir = player.position.x < centerTransform.position.x ? -1 : 1;
        FaceDirection(dir);
    }

    private void FaceDirection(int dir)
    {
        bool shouldFaceRight = dir > 0;

        if (facingRight == shouldFaceRight) return;

        facingRight = shouldFaceRight;

        Vector3 scale = visual.localScale;
        scale.x *= -1f;
        visual.localScale = scale;

        Vector3 pos = visual.localPosition;
        pos.x = facingRight ? 0f : -2.2f;
        visual.localPosition = pos;
    }

    // ========================================
    // HELPERS
    // ========================================
    private void PlayAnimation(int hash)
    {
        animator.CrossFade(hash, 0f, 0);
    }

    private void HandleDeath(EnemyStats stats)
    {
        if (isDead) return;
        ChangeState(State.Death);
    }

    // ========================================
    // VFX
    // ========================================
    public void ShowAttacksVFX()
    {
        GameObject fx = Instantiate(attacksVFX, visual.position, Quaternion.identity, visual);
        Animator anim = fx.GetComponent<Animator>();
        if (anim != null)
        {
            anim.enabled = false;
            anim.Play("attacks", 0, 0f);
            anim.enabled = true;
        }
        Destroy(fx, 1.167f);
    }

    public void ShowMeleeAttackVFX()
    {
        GameObject fx = Instantiate(attacksVFX, visual.position, Quaternion.identity, visual);
        Animator anim = fx.GetComponent<Animator>();
        if (anim != null)
        {
            anim.enabled = false;
            anim.Play("meleeAttack", 0, 0f);
            anim.enabled = true;
        }
        Destroy(fx, 0.417f);
    }

    public void ShowChargeVFX()
    {
        GameObject fx = Instantiate(chargeVFX, visual.position, Quaternion.identity, visual);
        Animator anim = fx.GetComponent<Animator>();
        if (anim != null)
        {
            anim.enabled = false;
            anim.Play("charge", 0, 0f);
            anim.enabled = true;
        }
        Destroy(fx, 1.167f);
    }
    public void ShowChargeBeamVFX()
    {
        GameObject fx = Instantiate(chargeBeamVFX, visual.position, Quaternion.identity, visual);
        Destroy(fx, 1.1f);
    }

    public void ShowChargeExplosionVFX()
    {
        GameObject fx = Instantiate(chargeExplosionVFX, visual.position, Quaternion.identity, visual);
        Destroy(fx, 1.1f);
    }

    public void GenerateBeam()
    {
        Instantiate(beam, new Vector2(player.position.x, -2.2f), Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        if (centerTransform == null) return;

        Vector3 center = centerTransform.position;

        // CLOSE RANGE (Combo Attack)
        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(center, center + Vector3.right * closeRange);
        // Gizmos.DrawLine(center, center + Vector3.left * closeRange);

        // MID RANGE (Charge Explosion)
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawLine(center, center + Vector3.right * midRange);
        // Gizmos.DrawLine(center, center + Vector3.left * midRange);

        // FAR RANGE (Charge Beam)
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(center, center + Vector3.right * farRange);
        Gizmos.DrawLine(center, center + Vector3.left * farRange);
    }
    [SerializeField] private CinemachineImpulseSource impulseSource;

    // Screen shake method called from animation event
    public void TriggerScreenShake()
    {
        if (impulseSource != null)
            ScreenShakeManager.Instance.ScreenShake(impulseSource);
    }
}