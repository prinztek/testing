using System;
using UnityEngine;

public class ShadowOfStormsBoss : MonoBehaviour
{
    // ========================================
    // STATES
    // ========================================
    public enum State
    {
        Idle,
        ChargeExplosion,
        ChargeBeam,
        ComboAttack,
        Vulnerable,
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
    [SerializeField] private float smallVulnerableTime = 0.4f;
    [SerializeField] private float bigVulnerableTime = 1f;

    // ========================================
    // ANIMATION HASHES
    // ========================================
    private static readonly int IdleHash = Animator.StringToHash("idle");
    private static readonly int DeathHash = Animator.StringToHash("dead");
    private static readonly int ChargeExplosionHash = Animator.StringToHash("chargeExplosion");
    private static readonly int ChargeBeamHash = Animator.StringToHash("chargeBeam");
    private static readonly int AttacksHash = Animator.StringToHash("attacks");

    // ========================================
    // INTERNAL
    // ========================================
    private State currentState;
    private float stateTimer;
    private int patternIndex;
    private bool isDead;
    private bool facingRight = true;

    private Transform player;
    private CharacterStats playerStats;

    // ========================================
    // MOVEMENT SETTINGS
    // ========================================
    [Header("Movement")]
    [SerializeField] private float chargeExplosionSpeed = 10f;
    [SerializeField] private float chargeBeamSpeed = 5f;
    [SerializeField] private float comboAttackSpeed = 7f;

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
        rb ??= GetComponent<Rigidbody2D>();

        ChangeState(State.Idle);
    }

    private void Update()
    {
        if (isDead) return;

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                if (stateTimer >= idleTime)
                    StartNextPatternAttack();
                break;

            case State.Vulnerable:
                float duration = (patternIndex == 2) ? bigVulnerableTime : smallVulnerableTime;

                if (stateTimer >= duration)
                    AdvancePattern();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        switch (currentState)
        {
            case State.ChargeExplosion:
                MoveTowardsPlayer(chargeExplosionSpeed);
                break;

            case State.ChargeBeam:
                // CHARGE FOR A SPELL CAST ATTACK (STATIONARY)
                // GRAB THE PLAYERS LAST POSITION
                // INSTANTIATE A BEAM IN THAT LAST POSITION
                // THIS IS CALLED THROUGH AN ANIMATION EVENT IN THE CHARGE BEAM ANIMATION
                break;

            case State.ComboAttack:
                MoveTowardsPlayer(comboAttackSpeed);
                break;

            default:
                // Stop movement for Idle, Vulnerable, Death
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    private void MoveTowardsPlayer(float speed)
    {
        if (player == null) return;

        float dir = player.position.x < centerTransform.position.x ? -1f : 1f;

        // Set velocity
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        // Optional: flip visuals while moving
        FacePlayer();
    }

    // ========================================
    // PATTERN CONTROL
    // ========================================
    private void StartNextPatternAttack()
    {
        switch (patternIndex)
        {
            case 0:
                ChangeState(State.ChargeExplosion);
                break;

            case 1:
                ChangeState(State.ChargeBeam);
                break;

            case 2:
                ChangeState(State.ComboAttack);
                break;
        }
    }

    private void AdvancePattern()
    {
        patternIndex++;

        if (patternIndex > 2)
            patternIndex = 0;

        ChangeState(State.Idle);
    }

    // ========================================
    // STATE MACHINE
    // ========================================
    private void ChangeState(State newState)
    {
        currentState = newState;
        stateTimer = 0f;

        // Only stop movement when Idle or Vulnerable
        if (newState == State.Idle || newState == State.Vulnerable)
            rb.linearVelocity = Vector2.zero;

        switch (newState)
        {
            case State.Idle:
                PlayAnimation(IdleHash);
                break;

            case State.ChargeExplosion:
                FacePlayer();
                PlayAnimation(ChargeExplosionHash);
                break;

            case State.ChargeBeam:
                FacePlayer();
                PlayAnimation(ChargeBeamHash);
                break;

            case State.ComboAttack:
                FacePlayer();
                PlayAnimation(AttacksHash);
                break;

            case State.Vulnerable:
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
    // ANIMATION EVENT CALLBACKS
    // ========================================
    public void OnChargeExplosionFinished()
    {
        ChangeState(State.Vulnerable);
    }

    public void OnChargeBeamFinished()
    {
        ChangeState(State.Vulnerable);
    }

    public void OnComboFinished()
    {
        ChangeState(State.Vulnerable);
    }

    // ========================================
    // VISUAL FLIP
    // ========================================
    private void FacePlayer()
    {
        if (player == null) return;

        int dir = player.position.x < centerTransform.position.x ? -1 : 1;
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

    public void Die()
    {
        if (isDead) return;
        ChangeState(State.Death);
    }

    // ========================================
    // YOUR ORIGINAL VFX (UNTOUCHED)
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

    public void ShowChargeExplosionVFX()
    {
        GameObject fx = Instantiate(chargeExplosionVFX, visual.position, Quaternion.identity, visual);
        Animator anim = fx.GetComponent<Animator>();
        if (anim != null)
        {
            anim.enabled = false;
            anim.Play("chargeExplosion", 0, 0f);
            anim.enabled = true;
        }
        Destroy(fx, 1.167f);
    }

    public void ShowChargeBeamVFX()
    {
        GameObject fx = Instantiate(chargeBeamVFX, visual.position, Quaternion.identity, visual);
        Animator anim = fx.GetComponent<Animator>();
        if (anim != null)
        {
            anim.enabled = false;
            anim.Play("chargeBeam", 0, 0f);
            anim.enabled = true;
        }
        Destroy(fx, 1.167f);
    }

    public void GenerateBeam()
    {
        Instantiate(beam, new Vector2(player.position.x, -2.2f), Quaternion.identity);
    }
}