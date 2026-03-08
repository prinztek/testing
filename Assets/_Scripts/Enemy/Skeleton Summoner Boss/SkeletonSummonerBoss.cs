using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSummonerBoss : MonoBehaviour
{
    // ========================
    // STATES / PHASES
    // ========================
    public enum BossPhase
    {
        Idle,
        Summon,
        Combat,
        Rage,
        Vulnerable,
        Death
    }

    [Header("References")]
    public EnemyStats enemyStats;
    public Animator animator;
    public Rigidbody2D rb;
    public Transform visual;
    public Transform centerTransform;
    public GameObject summonedSkeletonPrefab;
    public Transform summonPoint;
    [SerializeField] private GameObject meleeAttackVFX;
    [SerializeField] private GameObject summonAttackVFX;
    public Canvas healthBarCanvas; // reference to health bar for summonedHealthBarPrefab to set as parent when instantiated
    public GameObject summonedHealthBarPrefab; // reference to summoned minion health bar prefab

    [Header("Timing")]
    public float idleTime = 3f;
    public float summonDuration = 1.333f;
    public float rageDuration = 5f;
    public float vulnerableDuration = 5f;
    public float meleeDuration = 1.583f;
    public float meleeAttackCooldown = 2f; // FIX: separate cooldown so phaseTimer isn't corrupted

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rageMoveSpeed = 5f;
    public float attackRange = 1.5f;

    [Header("Combat")]
    public float minCombatDistance = 2f;

    [Header("Summon")]
    public int maxMinions = 3;
    private List<GameObject> activeMinions = new List<GameObject>();

    // ========================
    // STATE DATA
    // ========================
    private BossPhase currentPhase;
    private float phaseTimer;
    private float attackCooldownTimer; // FIX: dedicated attack timer, decoupled from phaseTimer
    private bool facingRight = true;
    private bool isDead;
    private bool isAttacking; // FIX: guard flag to prevent coroutine stacking

    [Header("Target")]
    [SerializeField] private Transform player; // FIX: added [SerializeField] so [Header] works and field is inspectable

    // ========================
    // Unity Methods
    // ========================

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;

        if (enemyStats != null)
            enemyStats.OnDeath += Die;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;

        if (enemyStats != null)
            enemyStats.OnDeath -= Die;
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        player = playerObj.transform;
    }

    private void Awake()
    {
        enemyStats ??= GetComponent<EnemyStats>();
        animator ??= GetComponentInChildren<Animator>();
        rb ??= GetComponent<Rigidbody2D>();

        ChangePhase(BossPhase.Idle);
    }

    private void Update()
    {
        if (isDead || player == null) return;

        phaseTimer += Time.deltaTime;
        attackCooldownTimer += Time.deltaTime; // FIX: always tick the dedicated attack cooldown

        switch (currentPhase)
        {
            case BossPhase.Idle:
                if (phaseTimer >= idleTime)
                    ChangePhase(BossPhase.Summon);
                break;

            case BossPhase.Summon:
                if (phaseTimer >= summonDuration)
                    ChangePhase(BossPhase.Combat);
                break;

            case BossPhase.Combat:
                CombatBehavior();
                activeMinions.RemoveAll(m => m == null);
                if (activeMinions.Count == 0)
                    ChangePhase(BossPhase.Rage);
                break;

            case BossPhase.Rage:
                RageBehavior();
                if (phaseTimer >= rageDuration)
                    ChangePhase(BossPhase.Vulnerable);
                break;

            case BossPhase.Vulnerable:
                if (phaseTimer >= vulnerableDuration)
                    ChangePhase(BossPhase.Idle);
                break;

            case BossPhase.Death:
                break;
        }
    }

    // ========================
    // Phase Management
    // ========================
    private void ChangePhase(BossPhase newPhase)
    {
        currentPhase = newPhase;
        phaseTimer = 0f;

        rb.linearVelocity = Vector2.zero;

        switch (newPhase)
        {
            case BossPhase.Idle:
                ResetSpriteColor();
                PlayAnimation("idle");
                break;

            case BossPhase.Summon:
                PlayAnimation("summon");
                // SummonMinions(); 
                break;

            case BossPhase.Combat:
                attackCooldownTimer = 0f;
                break;

            case BossPhase.Rage:
                attackCooldownTimer = 0f;
                PlayAnimation("run");
                break;

            case BossPhase.Vulnerable:
                ResetSpriteColor();
                PlayAnimation("vulnerable");
                break;

            case BossPhase.Death:
                ResetSpriteColor();
                PlayAnimation("dead");
                isDead = true;
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    // ========================
    // Combat Behaviors
    // ========================
    private void CombatBehavior()
    {
        // Lock all movement and facing during attack animation
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        FacePlayer();

        float distance = Vector2.Distance(player.position, centerTransform.position);

        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;

            if (attackCooldownTimer >= meleeAttackCooldown)
            {
                attackCooldownTimer = 0f;
                StartCoroutine(PlayMeleeAnimation());
            }
        }
        else if (distance > minCombatDistance)
        {
            MoveTowardPlayer(moveSpeed);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            PlayAnimation("idle");
        }
    }

    private void RageBehavior()
    {
        SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * 5f, 1f));

        // Lock all movement and facing during attack animation
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        FacePlayer();

        float distance = Vector2.Distance(player.position, centerTransform.position);

        if (distance > minCombatDistance)
            MoveTowardPlayer(rageMoveSpeed);

        if (distance <= attackRange && attackCooldownTimer >= meleeAttackCooldown)
        {
            attackCooldownTimer = 0f;
            StartCoroutine(DoubleMelee());
        }
    }

    private IEnumerator DoubleMelee()
    {
        yield return StartCoroutine(PlayMeleeAnimation());
        yield return StartCoroutine(PlayMeleeAnimation());
    }

    private IEnumerator PlayMeleeAnimation()
    {
        if (isAttacking) yield break;
        isAttacking = true;

        rb.linearVelocity = Vector2.zero;
        animator.Play("meleeAttack", 0, 0f); // force restart
        yield return new WaitForSeconds(meleeDuration);

        isAttacking = false;
    }

    // ========================
    // Movement & Facing
    // ========================
    private void MoveTowardPlayer(float speed)
    {
        int dir = player.position.x < centerTransform.position.x ? -1 : 1;
        FaceDirection(dir);

        float distance = Mathf.Abs(player.position.x - centerTransform.position.x);
        if (distance > minCombatDistance)
        {
            rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
            PlayAnimation("run");
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            PlayAnimation("idle");
        }
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

    // ========================
    // Summoning
    // ========================
    public void SummonMinions()
    {
        activeMinions.RemoveAll(m => m == null);
        // summon a single minion
        GameObject minion = Instantiate(
            summonedSkeletonPrefab,
            summonPoint.position,
            Quaternion.identity
        );

        activeMinions.Add(minion);

        GameObject healthBar = Instantiate(summonedHealthBarPrefab, healthBarCanvas.transform);
        var enemyStatsHealthBar = healthBar.GetComponent<EnemyStatsHealthBar>();
        enemyStatsHealthBar.enemyStats = minion.GetComponent<EnemyStats>();
        enemyStatsHealthBar.target = minion.transform;

        EnemyStats stats = minion.GetComponent<EnemyStats>();
        if (stats != null)
            stats.OnDeath += HandleMinionDeath; // FIX: subscription is kept, unsubscribed on death below
    }

    // FIX: unsubscribe from the dead minion's event to prevent memory leaks
    private void HandleMinionDeath(EnemyStats deadMinion)
    {
        deadMinion.OnDeath -= HandleMinionDeath;
        activeMinions.RemoveAll(m => m == null);
    }

    // ========================
    // Utilities
    // ========================
    private void PlayAnimation(string animName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            animator.Play(animName);
        }
    }

    // FIX: helper to cleanly reset the rage tint on phase exit
    private void ResetSpriteColor()
    {
        SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = Color.white;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void Die(EnemyStats stats)
    {
        if (isDead) return;
        ChangePhase(BossPhase.Death);
    }

    // ========================================
    // VFX
    // ========================================
    public void ShowMeleeAttackVFX()
    {
        GameObject fx = Instantiate(
            meleeAttackVFX,
            visual.position,
            Quaternion.identity,
            visual
        );

        Animator fxAnimator = fx.GetComponent<Animator>(); // FIX: renamed to avoid shadowing class field
        if (fxAnimator != null)
        {
            fxAnimator.enabled = false;
            fxAnimator.Play("meleeAttackVFX", 0, 0f);
            fxAnimator.enabled = true;
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

        Animator fxAnimator = fx.GetComponent<Animator>(); // FIX: renamed to avoid shadowing class field
        if (fxAnimator != null)
        {
            fxAnimator.enabled = false;
            fxAnimator.Play("summonAttackVFX", 0, 0f);
            fxAnimator.enabled = true;
        }

        Destroy(fx, 1.167f);
    }
}