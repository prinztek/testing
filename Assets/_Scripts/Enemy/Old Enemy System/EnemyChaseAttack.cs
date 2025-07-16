using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChaseAttack : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private EnemyStatsNew stats;
    private Animator animator;

    private enum State { Patrolling, Chasing, Attacking }
    private State currentState = State.Patrolling;

    [SerializeField] private GameObject attackCollider; // Assign 'attack1' here
    private float lastAttackTime = -999f;
    private bool isFacingRight = true;
    private bool isAttacking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<EnemyStatsNew>();
        animator = stats != null ? stats.Animator : GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null || stats == null || stats.IsDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float verticalDifference = Mathf.Abs(transform.position.y - player.position.y);

        switch (currentState)
        {
            case State.Patrolling:
                if (distanceToPlayer <= stats.DetectionRange && verticalDifference <= 1f)
                    currentState = State.Chasing;
                break;

            case State.Chasing:
                ChasePlayer();

                if (distanceToPlayer <= stats.AttackRange && verticalDifference <= 1f && Time.time >= lastAttackTime + stats.attackCooldown)
                    currentState = State.Attacking;

                else if (distanceToPlayer > stats.DetectionRange * 1.5f || verticalDifference > 2f)
                    currentState = stats.canPatrol ? State.Patrolling : State.Chasing;

                break;

            case State.Attacking:
                if (distanceToPlayer > stats.AttackRange || verticalDifference > 1f)
                {
                    currentState = State.Chasing;
                    isAttacking = false;
                    break;
                }

                if (Time.time >= lastAttackTime + stats.attackCooldown)
                    TryAttack();

                break;
        }

        // Enable/disable patrol script
        if (TryGetComponent(out EnemyPatrol patrol))
            patrol.enabled = (currentState == State.Patrolling);

        // Handle animation
        if (animator && currentState == State.Chasing && !animator.GetCurrentAnimatorStateInfo(0).IsName("enemyrunning"))
        {
            animator.CrossFade("enemyrunning", 0.1f);
        }
    }

    private void ChasePlayer()
    {
        FacePlayer();

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * stats.moveSpeed, rb.linearVelocity.y);
    }

    private void TryAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        rb.linearVelocity = Vector2.zero;
        FacePlayer();

        animator.CrossFade("enemyattack1", 0.1f);
        Invoke(nameof(ResetAttack), 0.5f); // Match your animation duration
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    private void FacePlayer()
    {
        if (player == null) return;
        Debug.Log("Flipped enemy: " + gameObject.name + " | New scale: " + transform.localScale);

        float direction = player.position.x - transform.position.x;

        if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;

            // Flip the main GameObject
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

            // Flip the attackCollider's localPosition X
            if (attackCollider != null)
            {
                Vector3 colliderPos = attackCollider.transform.localPosition;
                colliderPos.x *= -1;
                attackCollider.transform.localPosition = colliderPos;
            }
        }
    }

    // Animation Events
    public void EnableAttackCollider()
    {
        if (attackCollider != null)
            attackCollider.SetActive(true);
    }

    public void DisableAttackCollider()
    {
        if (attackCollider != null)
            attackCollider.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (stats == null)
            stats = GetComponent<EnemyStatsNew>();

        if (stats == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.AttackRange);
    }
}
