using System;
using UnityEngine;

public class ShadowOfStormsBoss : MonoBehaviour
{
    // ========================================
    // STATES
    // ========================================
    public enum State
    {
        Dormant,
        Idle,
        Approach,
        Attack,
        Recovery,
        Death
    }

    // ========================================
    // REFERENCES
    // ========================================
    [Header("References")]
    [SerializeField] public EnemyStats enemyStats;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform centerTransform;
    [SerializeField] private GameObject healVFX;
    [SerializeField] private GameObject attacksVFX;
    [SerializeField] private GameObject chargeVFX; // dash like
    [SerializeField] private GameObject chargeBeamVFX; // charging to shoot the beam
    [SerializeField] private GameObject chargeExplosionVFX; // charging to do the explosion

    // ========================================
    // TIMING
    // ========================================
    [Header("Timing")]
    [SerializeField] private float idleTime = 1f;
    [SerializeField] private float recoveryTime = 0.8f;

    // ========================================
    // MOVEMENT
    // ========================================
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRange = 1.5f;

    // ========================================
    // ANIMATION HASHES
    // ========================================
    private static readonly int IdleHash = Animator.StringToHash("idle");
    private static readonly int RunHash = Animator.StringToHash("run");
    private static readonly int DeathHash = Animator.StringToHash("dead");
    private static readonly int ChargeHash = Animator.StringToHash("charge"); // charging like a dash
    private static readonly int ChargeBeamHash = Animator.StringToHash("chargeBeam"); // charging to shoot the beam (CALL A BEAM VFX THROUGH ANIMATION EVENT)
    private static readonly int ChargeExplosionHash = Animator.StringToHash("chargeExplosion"); // charging to do the explosion
    private static readonly int TransitionHash = Animator.StringToHash("transition"); // for transition to charge
    private static readonly int AttacksHash = Animator.StringToHash("attacks"); // for attack variations

    // ========================================
    // STATE DATA
    // ========================================
    private State currentState;
    private float stateTimer;
    private bool facingRight = true;
    private bool isDead;

    // ========================================
    // TARGET
    // ========================================
    private Transform player;
    private CharacterStats playerStats;

    // ========================================
    // UNITY
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
        playerStats = player.GetComponent<CharacterStats>();
    }

    private void Awake()
    {
        enemyStats ??= GetComponent<EnemyStats>();
        animator ??= GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
                    if (InAttackRange())
                    {
                        // For now, just go back to idle when in range
                        // You can add attack state later
                        ChangeState(State.Attack);
                    }
                    else
                    {
                        ChangeState(State.Approach);
                    }
                }
                break;

            case State.Approach:
                MoveTowardPlayer();
                if (InAttackRange())
                    ChangeState(State.Attack);
                break;

            case State.Recovery:
                if (stateTimer >= recoveryTime)
                    ChangeState(State.Idle);
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

        rb.linearVelocity = Vector2.zero;

        switch (newState)
        {
            case State.Dormant:
                PlayAnimation(IdleHash);
                break;

            case State.Idle:
                PlayAnimation(IdleHash);
                break;

            case State.Approach:
                PlayAnimation(ChargeHash);
                break;

            case State.Attack:
                PlayAnimation(AttacksHash);
                break;

            case State.Recovery:
                PlayAnimation(IdleHash);
                break;

            case State.Death:
                isDead = true;
                PlayAnimation(DeathHash);
                break;
        }
    }

    // ========================================
    // MOVEMENT & DIRECTION
    // ========================================
    private void MoveTowardPlayer()
    {
        int dir = player.position.x < centerTransform.position.x ? -1 : 1;
        FaceDirection(dir);

        rb.linearVelocity = new Vector2(dir * enemyStats.GetMoveSpeed(), rb.linearVelocity.y);
    }

    private bool InAttackRange()
    {
        float distance = Mathf.Abs(player.position.x - centerTransform.position.x);
        return distance <= attackRange;
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
    // PUBLIC
    // ========================================
    public void Die()
    {
        if (isDead) return;
        ChangeState(State.Death);
    }

    // ========================================
    // HELPERS
    // ========================================
    private void PlayAnimation(int hash)
    {
        animator.CrossFade(hash, 0f, 0);
    }

    private void OnDrawGizmosSelected()
    {
        if (centerTransform == null) return;

        Vector3 center = centerTransform.position;

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);
    }

    // ========================================
    // VFX & ANIMATION EVENTS
    // ========================================
    public void ShowAttacksVFX()
    {
        GameObject fx = Instantiate(
            attacksVFX,
            visual.position,
            Quaternion.identity,
            visual
        );

        Animator animator = fx.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false; // Disable first
            animator.Play("attacks", 0, 0f); // Set animation
            animator.enabled = true; // Re-enable
        }

        Destroy(fx, 1.167f);
    }
}