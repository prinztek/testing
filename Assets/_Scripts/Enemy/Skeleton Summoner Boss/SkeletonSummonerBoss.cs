using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class SkeletonSummonerBoss : MonoBehaviour
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
        Summon,
        Recovery,
        Death
    }

    public enum BossAttackType
    {
        GroundSmash,      // ground spikes
        Summon,      // mid-range punish
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
    [SerializeField] private GameObject meleeAttackVFX;
    [SerializeField] private GameObject summonAttackVFX;
    [SerializeField] private List<Transform> summonPoints;
    [SerializeField] private GameObject summonedSkeletonPrefab;

    // ========================================
    // TIMING
    // ========================================
    [Header("Timing")]
    [SerializeField] private float idleTime = 1f;
    [SerializeField] private float attackDuration = 1.2f;
    [SerializeField] private float summonDuration = 1.5f;
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
    private static readonly int MeleeHash = Animator.StringToHash("meleeAttack");
    private static readonly int SummonHash = Animator.StringToHash("summon");
    private static readonly int DeathHash = Animator.StringToHash("dead");

    // ========================================
    // STATE DATA
    // ========================================
    private State currentState;
    public BossAttackType currentAttack;
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
                        ChangeState(State.Attack);
                    // ChangeState(Random.value < 0.5f ? State.Attack : State.Summon);
                    else
                    {
                        // randomly choose to approach or summon
                        int choice = UnityEngine.Random.Range(0, 2);
                        if (choice == 0)
                            ChangeState(State.Summon);
                        else
                            ChangeState(State.Approach);
                    }
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

            case State.Summon:
                if (stateTimer >= summonDuration)
                    ChangeState(State.Recovery);
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
                PlayAnimation(RunHash);
                break;

            case State.Attack:
                FacePlayer();
                PlayAttackAnimation();
                break;

            case State.Summon:
                FacePlayer();
                PlayAttackAnimation();
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

    private void PlayAttackAnimation()
    {
        if (currentState == State.Attack)
        {
            PlayAnimation(MeleeHash);
        }
        else if (currentState == State.Summon)
        {
            PlayAnimation(SummonHash);
        }
    }

    // called via animation event
    public void SummonSkeletons()
    {
        // pick randomly from the summon points
        // instantiate a minion prefab to hunt the player
        Instantiate(
            summonedSkeletonPrefab,
            summonPoints[0].position,
            Quaternion.identity
        );
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
        pos.x = facingRight ? 0f : -2f;
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

        // Close range (Spin)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);
    }

    public void ShowMeleeAttackVFX()
    {
        GameObject fx = Instantiate(
            meleeAttackVFX,
            visual.position,
            Quaternion.identity,
            visual
        );

        Animator animator = fx.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false; // Disable first
            animator.Play("meleeAttackVFX", 0, 0f); // Set animation
            animator.enabled = true; // Re-enable
        }

        Destroy(fx, 1.083f);
    }

    public void ShowSummonAttackVFX()
    {
        GameObject fx = Instantiate(
            summonAttackVFX,
            visual.position,
            Quaternion.identity,
            visual
        );

        Animator animator = fx.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false; // Disable first
            animator.Play("summonAttackVFX", 0, 0f); // Set animation
            animator.enabled = true; // Re-enable
        }

        Destroy(fx, 1.167f);
    }
}
