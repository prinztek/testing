using UnityEngine;
using System.Collections;

public class EnemyAIController : MonoBehaviour
{
    private EnemyStats stats;
    private EnemyBase enemyBase;

    [Header("Patrol Settings")]
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;

    private bool movingRight = true;
    private bool isPaused = false;

    public float groundCheckDistance = 0.3f;
    public float wallCheckDistance = 0.3f;
    public float pauseBeforeFlip = 1.5f;

    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        stats = GetComponent<EnemyStats>();
        enemyBase = GetComponent<EnemyBase>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>(); // ✅ Fix

        if (animator == null)
            Debug.LogWarning("❌ Animator not found on Slime or its children!");
    }

    void FixedUpdate()
    {
        if (enemyBase == null || !stats.canPatrol || isPaused)
            return;

        Patrol();
    }

    void Patrol()
    {
        float moveDir = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(moveDir * stats.moveSpeed, rb.linearVelocity.y);

        // Animation
        if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("running"))
            animator.CrossFade("running", 0.1f);

        // Debug rays
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.green);
        Debug.DrawRay(wallCheck.position, (movingRight ? Vector2.right : Vector2.left) * wallCheckDistance, Color.red);

        bool noGround = !Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        bool hitWall = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, wallCheckDistance, groundLayer);

        if (noGround || hitWall)
        {
            StartCoroutine(HandleFlip());
        }
    }

    IEnumerator HandleFlip()
    {
        isPaused = true;
        rb.linearVelocity = Vector2.zero;

        if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
            animator.CrossFade("idle", 0.1f);

        yield return new WaitForSeconds(pauseBeforeFlip);

        FlipDirection();

        if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("running"))
            animator.CrossFade("running", 0.1f);

        isPaused = false;
    }

    void FlipDirection()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {

        if (groundCheck)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }

        if (wallCheck)
        {
            Gizmos.color = Color.red;
            Vector3 dir = movingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * wallCheckDistance);
        }
    }
}
